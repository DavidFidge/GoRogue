﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace GoRogue.MapGeneration
{
    /// <summary>
    /// Raised by generation steps in <see cref="GenerationStep.OnPerform(GenerationContext)"/> when a parameter has been misconfigured.
    /// </summary>
    public class InvalidConfigurationException : Exception
    {
        /// <summary>
        /// Name of parameter that was misconfigured.
        /// </summary>
        public string? ParameterName { get; }

        /// <summary>
        /// Generation step that had a misconfigured parameter.
        /// </summary>
        public GenerationStep? GenerationStep { get; }

        /// <summary>
        /// Creates a configuration exception with a customized message.
        /// </summary>
        /// <param name="message"/>
        public InvalidConfigurationException(string message)
            : base(message) { }

        /// <summary>
        /// Creates a configuration exception with a customized message an inner exception.
        /// </summary>
        /// <param name="message"/>
        /// <param name="innerException"/>
        public InvalidConfigurationException(string message, Exception innerException)
            : base(message, innerException) { }

        /// <summary>
        /// Creates a configuration exception with a helpful message.
        /// </summary>
        /// <param name="generationStep">The generation step that the misconfigured parameter was encountered in.</param>
        /// <param name="parameterName">The name of the misconfigured parameter.</param>
        /// <param name="message">A message explaining the requirements for the parameter's value.</param>
        public InvalidConfigurationException(GenerationStep generationStep, string parameterName, string message)
            : base($"Invalid configuration encountered for generation step parameter:\n" +
                   $"    Generation Step: ${generationStep.GetType().Name} (name: {generationStep.Name}){message}\n" +
                   $"    Parameter Name : ${parameterName}\n" +
                   $"    Message        : ${message}")
        {
            ParameterName = parameterName;
            GenerationStep = generationStep;
        }

        /// <summary>
        /// Creates an empty configuration exception.
        /// </summary>
        public InvalidConfigurationException()
            : base() { }
    }

    /// <summary>
    /// Base class for implementing custom map generation steps.
    /// </summary>
    public abstract class GenerationStep
    {
        private readonly (Type type, string? tag)[] _requiredComponents;

        /// <summary>
        /// Components that are required and enforced to be on the <see cref="GenerationContext"/> when it is passed to <see cref="OnPerform(GenerationContext)"/>.
        /// Each component may optionally have a required tag.
        /// </summary>
        public IEnumerable<(Type type, string? tag)> RequiredComponents => _requiredComponents;

        /// <summary>
        /// The name of the generation step.
        /// </summary>
        public readonly string Name;

        // This constructor is required to remove ambiguous constructor call issues because both of the other ones use params
        /// <summary>
        /// Creates a generation step, optionally with a custom name.
        /// </summary>
        /// <param name="name">The name of the generation step being created.  Defaults to the name of the (runtime) class.</param>
        public GenerationStep(string? name = null)
            : this(name, Array.Empty<Type>()) { }

        /// <summary>
        /// Creates a generation step that requires the given component(s) on the <see cref="GenerationContext"/> to function.
        /// </summary>
        /// <param name="requiredComponents">Components that <see cref="OnPerform(GenerationContext)"/> will require from the context.</param>
        /// <param name="name">The name of the generation step being created.  Defaults to the name of the (runtime) class.</param>
        public GenerationStep(string? name = null, params Type[] requiredComponents)
            : this(name, requiredComponents.Select<Type, (Type, string?)>(i => (i, null)).ToArray()) { }

        /// <summary>
        /// Creates a generation step that requires the given component(s) on the <see cref="GenerationContext"/> to function.
        /// </summary>
        /// <param name="requiredComponents">Components that <see cref="OnPerform(GenerationContext)"/> will require from the context, and the tag
        /// required for each component.  Null means no particular tag is required.</param>
        /// <param name="name">The name of the generation step being created.  Defaults to the name of the (runtime) class.</param>
        public GenerationStep(string? name = null, params (Type type, string? tag)[] requiredComponents)
        {
            Name = name ?? GetType().Name;
            _requiredComponents = requiredComponents;
        }

        /// <summary>
        /// Performs the generation step on the given map context.  Throws exception if a required component is missing.
        ///
        /// This function is not virtual -- to implement actual geneation logic, implement <see cref="OnPerform(GenerationContext)"/>.
        /// </summary>
        /// <param name="context">Context to perform the generation step on.</param>
        public void PerformStep(GenerationContext context)
        {
            foreach (var (componentType, tag) in _requiredComponents)
            {
                if (!context.HasComponent(componentType, tag))
                    if (tag != null)
                        throw new InvalidOperationException($"Map generation step {GetType().Name} requires component of type {componentType.Name} with tag {tag} in the context it is given, but no component of that type with that tag was found.");
                    else
                        throw new InvalidOperationException($"Map generation step {GetType().Name} requires component of type {componentType.Name} in the context it is given, but no component of that type was found.");
            }

            OnPerform(context);
        }

        /// <summary>
        /// Implement to perform the actual work of the generation step.
        /// </summary>
        /// <param name="context">Context to perform the generation step on.</param>
        protected abstract void OnPerform(GenerationContext context);
    }
}
