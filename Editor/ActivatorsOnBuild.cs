using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace JanSharp
{
    [InitializeOnLoad]
    public static class ActivatorsOnBuild
    {
        static ActivatorsOnBuild()
        {
            OnBuildUtil.RegisterTypeCumulative<ActivatorBase>(OnBuild, order: 5);
        }

        private static Evaluator CreateEvaluator(ActivatorBase activator)
        {
            if (activator is ButtonActivator button)
                return new AlwaysFalseEvaluator(button);
            if (activator is ClockActivator clock)
                return new AlwaysFalseEvaluator(clock);
            if (activator is ItemTriggerActivator itemTrigger)
                return new ItemTriggerEvaluator(itemTrigger);
            if (activator is LogicalANDActivator logicalAND)
                return new LogicalANDEvaluator(logicalAND);
            if (activator is LogicalNOTActivator logicalNOT)
                return new LogicalNOTEvaluator(logicalNOT);
            if (activator is LogicalORActivator logicalOR)
                return new LogicalOREvaluator(logicalOR);
            if (activator is LogicalXORActivator logicalXOR)
                return new LogicalXOREvaluator(logicalXOR);
            if (activator is MemoryActivator memory)
                return new MemoryEvaluator(memory);
            if (activator is OnStartActivator onStart)
                return new AlwaysFalseEvaluator(onStart);
            if (activator is PlayerTriggerActivator playerTrigger)
                return new AlwaysFalseEvaluator(playerTrigger);
            if (activator is ToggleActivator toggleActivator)
                return new AlwaysFalseEvaluator(toggleActivator);
            throw new System.NotImplementedException($"Missing Evaluator for the activator type '{activator.GetType().FullName}'.");
        }

        private static string GetHierarchyPath(Transform t)
        {
            Transform parent = t.parent;
            if (parent == null)
                return t.name;
            return $"{GetHierarchyPath(parent)}/{t.name}";
        }

        private static bool OnBuild(IEnumerable<ActivatorBase> activators)
        {
            List<Evaluator> evaluators = new();
            Dictionary<ActivatorBase, Evaluator> evaluatorLut = new();
            foreach (ActivatorBase activator in activators)
            {
                Evaluator evaluator = CreateEvaluator(activator);
                evaluators.Add(evaluator);
                evaluatorLut.Add(activator, evaluator);
            }
            foreach (Evaluator evaluator in evaluators)
            {
                evaluator.ResolveInputs(evaluatorLut);
                evaluator.ResolveListeners(evaluatorLut);
            }
            EvaluatorRecursionDetector recursionDetector = new();
            // Doing a simple pass over all evaluators first, evaluating all states that do not involve any loops.
            // In doing so this ensures that the latter logic which performs infinite recursion checks has to
            // do less work - reducing the amount of evaluators which get reevaluated.
            foreach (Evaluator evaluator in evaluators)
                evaluator.Evaluate(recursionDetector, forceEvenIfInputIsNotEvaluated: false);
            foreach (Evaluator evaluator in evaluators)
            {
                if (evaluator.hasEvaluatedState)
                    continue;
                evaluator.Evaluate(recursionDetector, forceEvenIfInputIsNotEvaluated: true);
                if (recursionDetector.DetectedInfiniteRecursion)
                {
                    Debug.LogError(
                        $"[ActionActivatorSystem] There are activators listening to each other in a loop "
                            + $"where attempting to resolve their state results in an infinite loop. "
                            + $"Activators in question:\n"
                            + string.Join('\n', recursionDetector.InfinitelyRecursiveEvaluators
                                .Select(e => GetHierarchyPath(e.Activator.transform))),
                        recursionDetector.InfinitelyRecursiveEvaluators[0].Activator);
                    return false;
                }
            }
            foreach (Evaluator evaluator in evaluators)
            {
                // Doing this with just 2 SOs - one for setting the state to true and one for false - seems to
                // make Unity unhappy, presumably due to mixed target class types. It does work to some degree
                // but maybe some of the classes share the same field name with a different type, not sure,
                // didn't go look. There's definitely something that makes it unhappy in some cases.
                SerializedObject so = new SerializedObject(evaluator.Activator);
                so.FindProperty("state").boolValue = evaluator.state;
                if (evaluator.Activator is MemoryActivator)
                    so.FindProperty("syncedState").boolValue = evaluator.state;
                so.ApplyModifiedProperties();
            }
            return true;
        }
    }

    public class EvaluatorRecursionDetector
    {
        private List<Evaluator> stack = new();
        private Evaluator[] infinitelyRecursiveEvaluators = null;
        public Evaluator[] InfinitelyRecursiveEvaluators => infinitelyRecursiveEvaluators;
        public bool DetectedInfiniteRecursion => infinitelyRecursiveEvaluators != null;

        public void Push(Evaluator evaluator)
        {
            stack.Add(evaluator);
            evaluator.occurrencesOnStack++;
        }

        public void Pop()
        {
            Evaluator evaluator = stack[^1];
            stack.RemoveAt(stack.Count - 1);
            evaluator.occurrencesOnStack--;
        }

        public bool CheckForInfiniteRecursion()
        {
            Evaluator topEvaluator = stack[^1];
            int occurrences = topEvaluator.occurrencesOnStack;
            if (occurrences == 1)
                return false;
            int uncheckedCount = stack.Count - 1;
            int encounteredTopEvaluators = 1;
            while (encounteredTopEvaluators <= occurrences / 2)
            {
                uncheckedCount = stack.LastIndexOf(topEvaluator, uncheckedCount - 1);
                encounteredTopEvaluators++;
                if (IsRepeatedSequence(uncheckedCount))
                    return true;
            }
            return false;
        }

        private bool IsRepeatedSequence(int uncheckedCount)
        {
            // The found repeated topEvaluator is already considered "checked"
            // however it is not part of the potentially repeated sequence.
            int countNotPartOfSequence = uncheckedCount + 1;
            int sequenceLength = stack.Count - countNotPartOfSequence;
            int lowerSequenceStartIndex = countNotPartOfSequence - sequenceLength;
            if (lowerSequenceStartIndex < 0)
                return false;
            for (int i = lowerSequenceStartIndex; i < lowerSequenceStartIndex + sequenceLength; i++)
                if (stack[i] != stack[i + sequenceLength])
                    return false;
            infinitelyRecursiveEvaluators = new Evaluator[sequenceLength];
            stack.CopyTo(lowerSequenceStartIndex, infinitelyRecursiveEvaluators, 0, sequenceLength);
            return true;
        }
    }

    public abstract class Evaluator
    {
        public bool state = false;
        /// <summary>
        /// <para>As soon as this is <see langword="true"/>, <see cref="state"/> always has the valid
        /// final value.</para>
        /// </summary>
        public bool hasEvaluatedState = false;
        public abstract ActivatorBase Activator { get; }
        public List<Evaluator> listeners;

        public int occurrencesOnStack = 0;

        public abstract void ResolveInputs(Dictionary<ActivatorBase, Evaluator> evaluatorLut);

        protected Evaluator TryResolve(Dictionary<ActivatorBase, Evaluator> evaluatorLut, ActivatorBase activator)
            => activator != null && evaluatorLut.TryGetValue(activator, out var evaluator)
                ? evaluator
                : null;

        public void ResolveListeners(Dictionary<ActivatorBase, Evaluator> evaluatorLut)
        {
            listeners = (Activator.onActivateListeners ?? ActivatorEditorUtil.EmptyListeners)
                .Concat(Activator.onDeactivateListeners ?? ActivatorEditorUtil.EmptyListeners)
                .Concat(Activator.onStateChangedListeners ?? ActivatorEditorUtil.EmptyListeners)
                .Select(l => TryResolve(evaluatorLut, l as ActivatorBase)) // 'as' results in null if it cannot convert.
                .Where(e => e != null)
                .Distinct()
                .ToList();
        }

        public void Evaluate(EvaluatorRecursionDetector recursionDetector, bool forceEvenIfInputIsNotEvaluated)
        {
            bool prevState = state;
            bool hadEvaluatedValue = hasEvaluatedState;
            EvaluateCurrentState(forceEvenIfInputIsNotEvaluated);
            if (!hasEvaluatedState)
                return;
            if (hadEvaluatedValue && state == prevState)
                return;
            recursionDetector.Push(this);
            if (recursionDetector.CheckForInfiniteRecursion())
                return; // No need to pop, we are done.
            foreach (Evaluator listener in listeners)
            {
                listener.Evaluate(recursionDetector, forceEvenIfInputIsNotEvaluated);
                if (recursionDetector.DetectedInfiniteRecursion)
                    return; // No need to pop, we are done.
            }
            recursionDetector.Pop();
        }

        protected abstract void EvaluateCurrentState(bool forceEvenIfInputIsNotEvaluated);

        protected void SetEvaluatedState(bool state)
        {
            this.state = state;
            hasEvaluatedState = true;
        }
    }

    public class AlwaysFalseEvaluator : Evaluator
    {
        private ActivatorBase activator;
        public override ActivatorBase Activator => activator;

        public AlwaysFalseEvaluator(ActivatorBase activator) => this.activator = activator;
        public override void ResolveInputs(Dictionary<ActivatorBase, Evaluator> evaluatorLut) { }
        protected override void EvaluateCurrentState(bool forceEvenIfInputIsNotEvaluated)
            => SetEvaluatedState(false);
    }

    public class ItemTriggerEvaluator : Evaluator
    {
        private ItemTriggerActivator activator;
        public override ActivatorBase Activator => activator;
        public ItemTriggerEvaluator(ItemTriggerActivator activator) => this.activator = activator;
        public override void ResolveInputs(Dictionary<ActivatorBase, Evaluator> evaluatorLut) { }
        protected override void EvaluateCurrentState(bool forceEvenIfInputIsNotEvaluated)
            => SetEvaluatedState(activator.ItemCount != 0);
    }

    public class LogicalANDEvaluator : Evaluator
    {
        private LogicalANDActivator activator;
        public override ActivatorBase Activator => activator;
        private Evaluator[] inputs;

        public LogicalANDEvaluator(LogicalANDActivator activator) => this.activator = activator;

        public override void ResolveInputs(Dictionary<ActivatorBase, Evaluator> evaluatorLut)
        {
            inputs = activator.inputActivators
                .Select(a => TryResolve(evaluatorLut, a))
                .Where(i => i != null)
                .ToArray();
        }

        protected override void EvaluateCurrentState(bool forceEvenIfInputIsNotEvaluated)
        {
            if (!forceEvenIfInputIsNotEvaluated && inputs.Any(e => !e.hasEvaluatedState))
                return;
            SetEvaluatedState(inputs.All(e => e.state)); // When there are 0 inputs it will be true.
        }
    }

    public class LogicalNOTEvaluator : Evaluator
    {
        private LogicalNOTActivator activator;
        public override ActivatorBase Activator => activator;
        private Evaluator input;

        public LogicalNOTEvaluator(LogicalNOTActivator activator) => this.activator = activator;

        public override void ResolveInputs(Dictionary<ActivatorBase, Evaluator> evaluatorLut)
        {
            input = TryResolve(evaluatorLut, activator.inputActivator);
        }

        protected override void EvaluateCurrentState(bool forceEvenIfInputIsNotEvaluated)
        {
            if (!forceEvenIfInputIsNotEvaluated && input != null && !input.hasEvaluatedState)
                return;
            SetEvaluatedState(!(input?.state ?? false));
        }
    }

    public class LogicalOREvaluator : Evaluator
    {
        private LogicalORActivator activator;
        public override ActivatorBase Activator => activator;
        private Evaluator[] inputs;

        public LogicalOREvaluator(LogicalORActivator activator) => this.activator = activator;

        public override void ResolveInputs(Dictionary<ActivatorBase, Evaluator> evaluatorLut)
        {
            inputs = activator.inputActivators
                .Select(a => TryResolve(evaluatorLut, a))
                .Where(i => i != null)
                .ToArray();
        }

        protected override void EvaluateCurrentState(bool forceEvenIfInputIsNotEvaluated)
        {
            if (!forceEvenIfInputIsNotEvaluated && inputs.Any(e => !e.hasEvaluatedState))
                return;
            SetEvaluatedState(inputs.Any(e => e.state)); // When there are 0 inputs it will be false.
        }
    }

    public class LogicalXOREvaluator : Evaluator
    {
        private LogicalXORActivator activator;
        public override ActivatorBase Activator => activator;
        private Evaluator[] inputs;

        public LogicalXOREvaluator(LogicalXORActivator activator) => this.activator = activator;

        public override void ResolveInputs(Dictionary<ActivatorBase, Evaluator> evaluatorLut)
        {
            inputs = activator.inputActivators
                .Select(a => TryResolve(evaluatorLut, a))
                .Where(i => i != null)
                .ToArray();
        }

        protected override void EvaluateCurrentState(bool forceEvenIfInputIsNotEvaluated)
        {
            if (!forceEvenIfInputIsNotEvaluated && inputs.Any(e => !e.hasEvaluatedState))
                return;
            SetEvaluatedState(inputs.Count(e => e.state) == 1);
        }
    }

    public class MemoryEvaluator : Evaluator
    {
        private MemoryActivator activator;
        public override ActivatorBase Activator => activator;
        private Evaluator activate;
        private Evaluator reset;

        public MemoryEvaluator(MemoryActivator activator) => this.activator = activator;

        public override void ResolveInputs(Dictionary<ActivatorBase, Evaluator> evaluatorLut)
        {
            activate = TryResolve(evaluatorLut, activator.activateActivator);
            reset = TryResolve(evaluatorLut, activator.resetActivator);
        }

        protected override void EvaluateCurrentState(bool forceEvenIfInputIsNotEvaluated)
        {
            if (!forceEvenIfInputIsNotEvaluated
                && activate != null && !activate.hasEvaluatedState
                && reset != null && !reset.hasEvaluatedState)
            {
                return;
            }
            bool activateState = activate?.state ?? false;
            bool resetState = reset?.state ?? false;
            SetEvaluatedState(activateState && !resetState);
        }
    }
}
