using Rougelikeberry.Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rougelikeberry.Pathing
{
    public class Path
    {
        private readonly LinkedList<IBlock> _steps;
        private LinkedListNode<IBlock> _currentStep;

        /// <summary>
        /// The Block representing the first step or Start of this Path
        /// </summary>
        public IBlock Start { get { return _steps.First.Value; } }
        /// <summary>
        /// The Block representing the last step or End of this Path
        /// </summary>
        public IBlock End { get { return _steps.Last.Value; } }
        /// <summary>
        /// The number of steps in this Path
        /// </summary>
        public int Length { get { return _steps.Count; } }
        /// <summary>
        /// The Block represeting the step that is currently occupied in this Path
        /// </summary>
        public IBlock CurrentStep { get { return _currentStep.Value; } }
        /// <summary>
        /// All of the Blocks representing the Steps in this Path from Start to End as an IEnumerable
        /// </summary>
        public IEnumerable<IBlock> Steps { get { return _steps; } }

        /// <summary>
        /// Construct a new Path from the specified ordered list of Blocks
        /// </summary>
        /// <param name="steps">An IEnumerable of Blocks that represent the ordered steps along this Path from Start to End</param>
        public Path(IEnumerable<IBlock> steps)
        {
            _steps = new LinkedList<IBlock>(steps);

            if (_steps.Count < 1)
            {
                if (LogFilter.logError) { Debug.LogErrorFormat("Path must have steps."); }
            }

            _currentStep = _steps.First;
        }

        /// <summary>
        /// Move forward along this Path and advance the CurrentStep to the next Step in the Path
        /// </summary>
        public IBlock StepForward()
        {
            IBlock Block = TryStepForward();

            if (Block == null)
            {
                if (LogFilter.logWarn) { Debug.LogWarningFormat("Cannot take a step forward. End of path."); }
            }

            return Block;
        }

        /// <summary>
        /// Move forward along this Path and advance the CurrentStep to the next Step in the Path
        /// </summary>
        public IBlock TryStepForward()
        {
            LinkedListNode<IBlock> nextStep = _currentStep.Next;
            if (nextStep == null)
            {
                return null;
            }
            _currentStep = nextStep;
            return nextStep.Value;
        }

        /// <summary>
        /// Move backwards along this Path and rewind the CurrentStep to the previous Step in the Path
        /// </summary>
        public IBlock StepBackward()
        {
            IBlock Block = TryStepBackward();

            if (Block == null)
            {
                if (LogFilter.logWarn) { Debug.LogWarningFormat("Cannot take a step backward. Start of path."); }
            }

            return Block;
        }

        /// <summary>
        /// Move backwards along this Path and rewind the CurrentStep to the next Step in the Path
        /// </summary>
        public IBlock TryStepBackward()
        {
            LinkedListNode<IBlock> previousStep = _currentStep.Previous;
            if (previousStep == null)
            {
                return null;
            }
            _currentStep = previousStep;
            return previousStep.Value;
        }
    }
}