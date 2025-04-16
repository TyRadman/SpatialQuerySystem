# Spatial Query System for Unity

<p>
  <img src="https://img.shields.io/badge/Unity-000000?logo=unity&logoColor=white&style=for-the-badge" height="40">
  <img src="https://img.shields.io/badge/C%23-239120?logo=csharp&logoColor=white&style=for-the-badge" height="40">
</p>

A flexible and extensible spatial query framework for Unity. Designed for AI and gameplay systems that require evaluating positions in space based on customizable rules and criteria.

## Features

- Custom inspector tooling.
- Extensible via ScriptableObjects with no additional setups.
- Modular and scalable architecture that users to add custom evaluators, generators, and query subjects.
- Debug mode and sample point visualizers.
- Automatic editor integration for all custom class (like evaluators, generators, and subjects).
- 5 evaluators and 4 generators.

## Usage
1. Create a `SpatialQueryAsset` asset through the menu.

![image](https://github.com/user-attachments/assets/323d4064-e595-4c35-815e-d56be02c09df)

3. Select a directory, name the asset, and save it.
4. In the inspector, assign a generator type.

![image](https://github.com/user-attachments/assets/f94d5935-71c8-48b6-ac82-6d6182da2155)

5. Select an evaluator/s.

![image](https://github.com/user-attachments/assets/358a5ca4-0abb-4f1c-a886-4628ad14e8bc)

6. Customize values that work best for you game!

![image](https://github.com/user-attachments/assets/33028940-7c55-4c9d-97be-a006ed05604d)

7. Reference the `SpatialQueryAsset` in the AI entity that is going to use it. Initiate it by passing the transform of the entity using the asset.

```C#
[SerializeField] private SpatialQueryAsset _spatialQueryAsset;

// using awake here for the sake of demonstration. Use any other setup methods that you might have.
private void Awake()
{
  _spatialQueryAsset.Initiate(transform);
}
```

8. Call `GetPointPosition()` to get the position of the point that match rules of the evaluators provided.

```C#
private void SomeMovementMethod()
{
  Vector3 positionToMoveTo = _spatialQueryAsset.GetPointPosition();
}
```

NOTE: you might need to pass a reference to the target if the evaluator subject type is set to `Target`. More on that next.

## Evaluators

Evaluators define the rules used to score or filter sampled points. Each one inherits from `SpatialQueryEvaluator` and implements custom logic for determining relevance based on a `query context` and `subject`.

Evaluators support three scoring modes (`Score`, `Filter`, `FilterAndScore`) and can be extended with minimal boilerplate.

### Base Methods & Properties

| Member | Type | Description |
|--------|------|-------------|
| `ScoringMode` | `ScoringMode` | Defines how the evaluator behaves: scoring, filtering, or both. |
| `FilteringScore` | `Vector2` | Score range used when filtering is enabled. Points within this range may be excluded. |
| `ScoreWeight` | `float` | Multiplier applied to the final score added by this evaluator. |
| `Init(Transform owner)` | `virtual void` | Optional setup before evaluation begins. Use to cache data or prepare state. |
| `Evaluate(SpatialEvaluationContext context)` | `virtual void` | Main evaluation method that loops over sample points and scores them. |
| `SetSamplePointScore(...)` | `abstract void` | Override in child classes to define how each point is scored. |
| `AddToScore(point, value)` | `void` | Adds weighted score to a point and handles filtering logic. This is the only way to register the evaluation score. |
| `GetSubjectPosition(...)` | `Vector2` | Returns the position of the evaluated subject (`Querier`, `Target`, or `Custom`). |

### Editor Methods and Properties

| Member | Type | Description |
|--------|------|-------------|
| `GetEvaluatorSummary()` | `virtual string` | Returns a string summary for display in the inspector. |
| `GetIconPath()` | `virtual string` | Returns a custom icon path for UI (optional). |

### Customization

A custom evaluator can be create by override `SpatialQueryEvaluator` or by going to the menu, then select `Spatial Query -> New Custom Script -> Evaluator`. The generated script will look like this:
``` C#
namespace SpatialQuery
{                    
    public class EvluatorClassName : SpatialQueryEvaluator
    {
        protected override void SetSamplePointScore(SpatialEvaluationContext context, SpatialQuerySamplePoint samplePoint)
        {
            // implement scoring logic here

            // Must register the score by using AddToScore()
            // AddToScore(samplePoint, scoreToAdd);
        }

        // (Optional) for custom evaluator icon in the editor, place the icon in SpatialQuery/Textures/ and override GetIconPath()
        public override string GetIconPath()
        {
            return "T_EvaluatorIcon.png";
        }

        // (Optional) to customize details display in the editor, override GetEvaluatorSummary()
        public override string GetEvaluatorSummary()
        {
            float someData = 0f;
            return $"Some data: {someData}";
        }
    }
}
```

## Generators

Generators define how spatial sample points are created in the world. Each generator inherits from `SpatialQueryGenerator` and implements a strategy for producing positions to be evaluated.

Generators can use the `NavMesh` to ensure points are reachable and valid for AI navigation.

### Base Methods & Properties

| Member | Type | Description |
|--------|------|-------------|
| `GenerateSamplePoints(context)` | `virtual void` | Override in derived classes to return a list of valid `SpatialQuerySamplePoint` objects based on the provided context. |
| `_querier` | `Transform` | The entity requesting the spatial query; set automatically when `GenerateSamplePoints` is called. |
| `_target` | `Transform` | The query's target (if applicable); also set via context. |
| `IsPointValid(source, point)` | `protected bool` | Returns `true` if a valid NavMesh path exists from `source` to `point`. |

### Customization

Custom generators can be created by inheriting from `SpatialQueryGenerator`, or by selecting  
`Spatial Query -> New Custom Script -> Generator` from the menu.

The generated script looks like this:

```C#
namespace SpatialQuery
{
    public class GeneratorClassName : SpatialQueryGenerator
    {
        public override List<SpatialQuerySamplePoint> GenerateSamplePoints(SpatialSamplingContext context)
        {
            List<SpatialQuerySamplePoint> points = new List<SpatialQuerySamplePoint>();

            // Example: generate a single point in front of the querier
            Vector3 forwardPoint = context.Querier.position + context.Querier.forward * 2f;
            points.Add(new SpatialQuerySamplePoint(forwardPoint));

            return points;
        }
    }
}
```

Use IsPointValid() and GetPathLength() for NavMesh validation and path-based logic when necessary.

## Custom Evaluator Subjects

Custom subjects allow evaluators to reference entities other than the querier or target — useful for cases like group coordination, custom targets or dynamically chosen references.

All custom subjects inherit from `SpatialQueryEvaluatorSubjectBase`.

### Base Methods & Properties

| Member | Type | Description |
|--------|------|-------------|
| `SetUp(owner)` | `virtual` | Assigns the owner GameObject. Called automatically before evaluation begins. |
| `GetEvaluatorSubject()` | `abstract Transform` | Must return a `Transform` representing the subject to be evaluated. |
| `_owner` | `GameObject` | Internally stored reference to the owner GameObject. |

### Customization

Custom subjects can be created by inheriting from `SpatialQueryEvaluatorSubjectBase`, or by selecting  
`Spatial Query -> New Custom Script -> Subject` from the menu.

The generated script looks like this:

```csharp
namespace SpatialQuery
{
    public class SubjectClassName : SpatialQueryEvaluatorSubjectBase
    {
        public override Transform GetEvaluatorSubject()
        {
            // Return the transform of the subject to evaluate against
            return _owner.transform; // example: return owner itself
        }
    }
}
```
## Future improvements

- **Asynchronous Evaluation Support** – Offload heavy queries to background threads.
- **In-Edit Query Testing Tool** – Visual tool for simulating queries and previewing results.
- **Runtime Configuration** – Load and adjust query settings dynamically through code or UI.
- **Sample Points Caching** - Cache sampled points to avoid repeating evaluations in the future.
