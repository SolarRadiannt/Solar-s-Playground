using Godot;
using System;
using System.Collections.Generic;

namespace GodotUtilities;

public class WeightedTable<T>(RandomNumberGenerator rng = null)
{
    private record Entry(T Item, float Weight);

    private readonly RandomNumberGenerator rng = rng ?? new();
    private readonly List<Entry> entries = new();
    private float total = 0f;

    public int Count => entries.Count;

    /// <summary>
    /// Adds an item with the given <paramref name="weight"/> to the table.<br/>
    /// Higher weight relative to the total = higher probability of being picked.<br/>
    /// Duplicate items are allowed — their weights stack in <see cref="GetProbability"/>.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="weight"/> is zero or negative.</exception>
    public WeightedTable<T> Add(T item, float weight)
    {
        if (weight <= 0f)
            throw new ArgumentOutOfRangeException(nameof(weight), "Weight must be greater than zero.");
        entries.Add(new(item, weight));
        total += weight;
        return this;
    }

    /// <summary>
    /// Removes the first entry matching <paramref name="item"/> from the table.
    /// </summary>
    /// <returns><see langword="true"/> if an entry was found and removed; <see langword="false"/> otherwise.</returns>
    public bool Remove(T item)
    {
        for (int i = 0; i < entries.Count; i++)
        {
            if (!EqualityComparer<T>.Default.Equals(entries[i].Item, item))
                continue;

            total -= entries[i].Weight;
            entries.RemoveAt(i);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Picks a random item, weighted by each entry's relative weight.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the table is empty.</exception>
    public T Pick()
    {
        if (entries.Count == 0)
            throw new InvalidOperationException($"Can't pick from an empty WeightedTable<{typeof(T).Name}>.");

        float roll = rng.RandfRange(0f, total);
        float acc  = 0f;

        foreach (var entry in entries)
        {
            acc += entry.Weight;
            if (roll < acc)
                return entry.Item;
        }

        // Fallback: floating point drift can cause the loop to exhaust without a match.
        // The last entry is the statistically correct winner in this case.
        return entries[^1].Item;
    }

    /// <summary>
    /// Yields <paramref name="count"/> picks with replacement — the same item can appear multiple times.
    /// </summary>
    public IEnumerable<T> PickMany(int count)
    {
        for (int i = 0; i < count; i++)
            yield return Pick();
    }

    /// <summary>
    /// Picks <paramref name="count"/> distinct items without replacement, respecting weights.<br/>
    /// Each pick removes the winner from a temporary pool, so heavier items are still
    /// more likely but can only appear once.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="count"/> exceeds the number of entries.</exception>
    public IEnumerable<T> PickManyUnique(int count)
    {
        if (count > entries.Count)
            throw new ArgumentOutOfRangeException(nameof(count),
                $"Requested {count} unique items but table only has {entries.Count} entries.");

        // Work on a temporary copy so the original table is not modified.
        var pool      = new List<Entry>(entries);
        float poolTotal = total;

        for (int i = 0; i < count; i++)
        {
            float roll = rng.RandfRange(0f, poolTotal);
            float acc  = 0f;

            for (int j = 0; j < pool.Count; j++)
            {
                acc += pool[j].Weight;
                if (roll < acc)
                {
                    yield return pool[j].Item;
                    poolTotal -= pool[j].Weight;
                    pool.RemoveAt(j);
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Returns the probability of <paramref name="item"/> being picked as a value between 0 and 1.<br/>
    /// Sums weight across duplicate entries.<br/>
    /// Returns <see langword="null"/> if the item is not in the table.
    /// </summary>
    public float? GetProbability(T item)
    {
        float weight = 0f;
        bool found   = false;

        foreach (var entry in entries)
        {
            if (!EqualityComparer<T>.Default.Equals(entry.Item, item))
                continue;
            weight += entry.Weight;
            found   = true;
        }

        return found ? weight / total : null;
    }

    /// <summary>Removes all entries and resets the table.</summary>
    public void Clear()
    {
        entries.Clear();
        total = 0f;
    }
}

