using FribaScore.Bui.Models;

namespace FribaScore.Bui.Data;

/// <summary>
/// Contains dummy data for disc golf courses and holes, used for testing and development purposes.
/// </summary>
public static class DummyData
{
    /// <summary>
    /// Dummy list of disc golf courses with their respective holes, pars, and lengths.
    /// </summary>
    public static List<Course> Courses { get; } = new List<Course>
    {
        new Course
        {
            Identifier = "one-in-hole",
            Name = "One Hole Wonder",
            Holes = new List<Hole>
            {
                new Hole { Number = 1, Par = 1, Length = 50 }
            }
        },
        new Course
        {
            Identifier = "kivenlahti",
            Name = "Kivenlahti Frisbeegolf",
            Holes = new List<Hole>
            {
                new Hole { Number = 1, Par = 3, Length = 120 },
                new Hole { Number = 2, Par = 3, Length = 95 },
                new Hole { Number = 3, Par = 4, Length = 145 },
                new Hole { Number = 4, Par = 3, Length = 88 },
                new Hole { Number = 5, Par = 4, Length = 130 },
                new Hole { Number = 6, Par = 3, Length = 100 },
                new Hole { Number = 7, Par = 3, Length = 110 },
                new Hole { Number = 8, Par = 4, Length = 155 },
                new Hole { Number = 9, Par = 3, Length = 92 }
            }
        },
        new Course
        {
            Identifier = "tali",
            Name = "Tali Disc Golf Park",
            Holes = new List<Hole>
            {
                new Hole { Number = 1, Par = 3, Length = 105 },
                new Hole { Number = 2, Par = 4, Length = 140 },
                new Hole { Number = 3, Par = 3, Length = 90 },
                new Hole { Number = 4, Par = 3, Length = 115 },
                new Hole { Number = 5, Par = 4, Length = 125 },
                new Hole { Number = 6, Par = 3, Length = 85 },
                new Hole { Number = 7, Par = 3, Length = 98 },
                new Hole { Number = 8, Par = 3, Length = 108 },
                new Hole { Number = 9, Par = 4, Length = 135 }
            }
        },
        new Course
        {
            Identifier = "laajis",
            Name = "Laajis 9-reikäinen",
            Holes = new List<Hole>
            {
                new Hole { Number = 1, Par = 3, Length = 100 },
                new Hole { Number = 2, Par = 3, Length = 85 },
                new Hole { Number = 3, Par = 4, Length = 130 },
                new Hole { Number = 4, Par = 3, Length = 95 },
                new Hole { Number = 5, Par = 3, Length = 110 },
                new Hole { Number = 6, Par = 4, Length = 120 },
                new Hole { Number = 7, Par = 3, Length = 88 },
                new Hole { Number = 8, Par = 3, Length = 102 },
                new Hole { Number = 9, Par = 4, Length = 115 }
            }
        },
        new Course         {
            Identifier = "meilahti",
            Name = "Meilahden Puisto",
            Holes = new List<Hole>
            {
                new Hole { Number = 1, Par = 3, Length = 90 },
                new Hole { Number = 2, Par = 3, Length = 75 },
                new Hole { Number = 3, Par = 3, Length = 95 },
                new Hole { Number = 4, Par = 4, Length = 115 },
                new Hole { Number = 5, Par = 3, Length = 82 },
                new Hole { Number = 6, Par = 3, Length = 88 },
                new Hole { Number = 7, Par = 3, Length = 100 },
                new Hole { Number = 8, Par = 4, Length = 125 },
                new Hole { Number = 9, Par = 3, Length = 92 }
            }
        }
    };
}
