using TraineeTracker.Data.ApplicationUsers;
using TraineeTracker.Data.Feedbacks;
using TraineeTracker.Data.Lessons;
using TraineeTracker.Data.TeachingPlans;
using TraineeTracker.Data.TraineeLessonLog;
using TraineeTracker.Data.TraineeLessons;
using TraineeTracker.Models.Domain;
using TraineeTracker.Models.Dtos;
using TraineeTracker.Services.Admin;

namespace TraineeTracker.Services.Seeders {

    /// <summary>
    /// Service responsible for seeding test data for lessons, users, feedback, and progress states.
    /// Used during development and E2E testing to populate the database with predefined content.
    /// </summary>
    /// <remarks>Code Ownership: Simon Hinterreiter (hintsimo)</remarks>
    public class TestDataSeeder {

        /// <summary>
        /// Provides functionality for creating and managing users during the seeding process.
        /// </summary>
        private readonly AdminService _adminService;

        /// <summary>
        /// Repository for accessing and modifying application users in the database.
        /// </summary>
        private readonly IApplicationUserRepository _databaseApplicationUserRepository;

        /// <summary>
        /// Repository for accessing and modifying lesson entities in the database.
        /// </summary>
        private readonly ILessonRepository _databaseLessonRepository;

        /// <summary>
        /// Repository for accessing and modifying trainee lesson entities in the database.
        /// </summary>
        private readonly ITraineeLessonRepository _databaseTraineeLessonRepository;

        /// <summary>
        /// Repository for accessing and modifying teaching plans in the database.
        /// </summary>
        private readonly ITeachingPlanRepository _databaseTeachingPlanRepository;

        /// <summary>
        /// Repository for accessing and storing feedback entries associated with lessons.
        /// </summary>
        private readonly IFeedbackRepository _databaseFeedbackRepository;

        /// <summary>
        /// Repository for storing and retrieving lesson log entries for state transitions and audits.
        /// </summary>
        private readonly ITraineeLessonLogEntryRepository _databaseTraineeLessonLogEntryRepository;

        // ---------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the <see cref="TestDataSeeder"/> class 
        /// with all required repository and service dependencies.
        /// </summary>
        public TestDataSeeder(IApplicationUserRepository databaseApplicationUserRepository, ILessonRepository lessonRepo, ITraineeLessonRepository traineeLessonRepo, ITeachingPlanRepository teachingPlanRepo, AdminService adminService, IFeedbackRepository feedbackRepo, ITraineeLessonLogEntryRepository logEntryRepo, TraineeStatisticsService statisticsService) {
            _databaseApplicationUserRepository = databaseApplicationUserRepository;
            _databaseLessonRepository = lessonRepo;
            _databaseTeachingPlanRepository = teachingPlanRepo;
            _adminService = adminService;
            _databaseTraineeLessonRepository = traineeLessonRepo;
            _databaseFeedbackRepository = feedbackRepo;
            _databaseTraineeLessonLogEntryRepository = logEntryRepo;
        }

        // ---------------------------------------------------
        /// <summary>
        /// Seeds a predefined set of lessons into the database if they do not already exist.
        /// Includes both generic and Makandra JSON-based lessons.
        /// </summary>
        public async Task SeedLessonsAsync() {
            var lessons = new List<Lesson>();

            for (int i = 1; i <= 80; i++) {
                lessons.Add(
                    new Lesson {
                        TeachingPlanId = i > 40 ? 2 : 1,
                        SortingIndex = i - 1, // 0-basiert
                        MakandraId = (200 + i).ToString(), // "201" bis "280"
                        Title = $"Lesson {i}: {(i <= 40 ? "WebDev" : "DevOps")} Topic {i} (Only {(i <= 40 ? "WebDev" : "DevOps")})",
                        EstimatedEffort = (i % 4) switch {
                            1 => 2.0,
                            2 => 2.5,
                            3 => 3.0,
                            _ => 1.5
                        },
                        LinkUrl = (i % 4) switch {
                            1 => "https://learn.microsoft.com/de-de/dotnet/csharp/fundamentals/object-oriented/",
                            2 => "https://learn.microsoft.com/de-de/ef/core/",
                            3 => "https://learn.microsoft.com/de-de/aspnet/core/mvc/",
                            _ => "https://learn.microsoft.com/de-de/dotnet/csharp/"
                        }
                    });
            }

            // ++++++++++++++++
            // WebDevJSON
            var lessons2 = new List<Lesson> {
                new Lesson {
                    MakandraId = "34973",
                    Title = "000 Start here!",
                    LinkUrl = "https://makandracards.com/curriculum/34973-start-0d",
                    EstimatedEffort = 0.0,
                    IsInactive = false,
                    SortingIndex = 0,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "34329",
                    Title = "105 Ruby basics",
                    LinkUrl = "https://makandracards.com/curriculum/34329-ruby-basics-2d",
                    EstimatedEffort = 2.0,
                    IsInactive = false,
                    SortingIndex = 1,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "35167",
                    Title = "110 Where to find API documentation",
                    LinkUrl = "https://makandracards.com/curriculum/35167-find-api-documentation-0-5d",
                    EstimatedEffort = 0.5,
                    IsInactive = false,
                    SortingIndex = 2,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "34335",
                    Title = "120 Git basics",
                    LinkUrl = "https://makandracards.com/curriculum/34335-git-basics-1d",
                    EstimatedEffort = 1.0,
                    IsInactive = false,
                    SortingIndex = 3,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "34333",
                    Title = "125 Gems, bundler, rbenv",
                    LinkUrl = "https://makandracards.com/curriculum/34333-gems-bundler-rbenv-1d",
                    EstimatedEffort = 1.0,
                    IsInactive = false,
                    SortingIndex = 4,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "58436",
                    Title = "128 Sample apps",
                    LinkUrl = "https://makandracards.com/curriculum/58436-sample-apps-0-25d",
                    EstimatedEffort = 0.25,
                    IsInactive = false,
                    SortingIndex = 5,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "34337",
                    Title = "130 Ruby on Rails basics",
                    LinkUrl = "https://makandracards.com/curriculum/34337-ruby-rails-basics-4d",
                    EstimatedEffort = 4.0,
                    IsInactive = false,
                    SortingIndex = 6,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "34339",
                    Title = "135 SQL basics",
                    LinkUrl = "https://makandracards.com/curriculum/34339-sql-basics-2-5d",
                    EstimatedEffort = 2.5,
                    IsInactive = false,
                    SortingIndex = 7,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "34341",
                    Title = "140 Testing basics",
                    LinkUrl = "https://makandracards.com/curriculum/34341-testing-basics-3-5d",
                    EstimatedEffort = 3.5,
                    IsInactive = false,
                    SortingIndex = 8,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "508439",
                    Title = "141 Creating test data with factories",
                    LinkUrl = "https://makandracards.com/curriculum/508439-creating-test-data-factories-1d",
                    EstimatedEffort = 1.0,
                    IsInactive = false,
                    SortingIndex = 9,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "505778",
                    Title = "142 Validations",
                    LinkUrl = "https://makandracards.com/curriculum/505778-validations-1d",
                    EstimatedEffort = 1.0,
                    IsInactive = false,
                    SortingIndex = 10,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "34343",
                    Title = "145 CSS basics",
                    LinkUrl = "https://makandracards.com/curriculum/34343-css-basics-3d",
                    EstimatedEffort = 3.0,
                    IsInactive = false,
                    SortingIndex = 11,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "34347",
                    Title = "150 JavaScript basics",
                    LinkUrl = "https://makandracards.com/curriculum/34347-javascript-basics-2d",
                    EstimatedEffort = 2.0,
                    IsInactive = false,
                    SortingIndex = 12,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "508438",
                    Title = "152 Working with the DOM",
                    LinkUrl = "https://makandracards.com/curriculum/508438-working-dom-1-5d",
                    EstimatedEffort = 1.5,
                    IsInactive = false,
                    SortingIndex = 13,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "34385",
                    Title = "154 Haml",
                    LinkUrl = "https://makandracards.com/curriculum/34385-haml-0-5d",
                    EstimatedEffort = 0.5,
                    IsInactive = false,
                    SortingIndex = 14,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "43384",
                    Title = "162 Personal security",
                    LinkUrl = "https://makandracards.com/curriculum/43384-personal-security-0-5d",
                    EstimatedEffort = 0.5,
                    IsInactive = false,
                    SortingIndex = 15,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "34389",
                    Title = "165 ActiveRecord scopes",
                    LinkUrl = "https://makandracards.com/curriculum/34389-activerecord-scopes-4d",
                    EstimatedEffort = 4.0,
                    IsInactive = false,
                    SortingIndex = 16,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "34349",
                    Title = "167 Software design basics",
                    LinkUrl = "https://makandracards.com/curriculum/34349-software-design-basics-4d",
                    EstimatedEffort = 4.0,
                    IsInactive = false,
                    SortingIndex = 17,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "34391",
                    Title = "170 Memoization",
                    LinkUrl = "https://makandracards.com/curriculum/34391-memoization-0-4d",
                    EstimatedEffort = 0.4,
                    IsInactive = false,
                    SortingIndex = 18,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "561989",
                    Title = "171 Acceptance testing with Cucumber",
                    LinkUrl = "https://makandracards.com/curriculum/561989-acceptance-testing-cucumber-2d",
                    EstimatedEffort = 2.0,
                    IsInactive = false,
                    SortingIndex = 19,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "60267",
                    Title = "172 Debugging",
                    LinkUrl = "https://makandracards.com/curriculum/60267-debugging-1d",
                    EstimatedEffort = 1.0,
                    IsInactive = false,
                    SortingIndex = 20,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "35613",
                    Title = "175 RSpec in depth",
                    LinkUrl = "https://makandracards.com/curriculum/35613-rspec-depth-2d",
                    EstimatedEffort = 2.0,
                    IsInactive = false,
                    SortingIndex = 21,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "34407",
                    Title = "180 Personal productivity",
                    LinkUrl = "https://makandracards.com/curriculum/34407-personal-productivity-1d",
                    EstimatedEffort = 1.0,
                    IsInactive = false,
                    SortingIndex = 22,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "34351",
                    Title = "182 Deployment basics",
                    LinkUrl = "https://makandracards.com/curriculum/34351-deployment-basics-1d",
                    EstimatedEffort = 1.0,
                    IsInactive = false,
                    SortingIndex = 23,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "35165",
                    Title = "185 Our process",
                    LinkUrl = "https://makandracards.com/curriculum/35165-process-2d",
                    EstimatedEffort = 2.0,
                    IsInactive = false,
                    SortingIndex = 24,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "34397",
                    Title = "186 Linux basics",
                    LinkUrl = "https://makandracards.com/curriculum/34397-linux-basics-1d",
                    EstimatedEffort = 1.0,
                    IsInactive = false,
                    SortingIndex = 25,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "43387",
                    Title = "187 Exception notifications",
                    LinkUrl = "https://makandracards.com/curriculum/43387-exception-notifications-0-5d",
                    EstimatedEffort = 0.5,
                    IsInactive = false,
                    SortingIndex = 26,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "34387",
                    Title = "190 Pagination",
                    LinkUrl = "https://makandracards.com/curriculum/34387-pagination-0-5d",
                    EstimatedEffort = 0.5,
                    IsInactive = false,
                    SortingIndex = 27,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "35281",
                    Title = "200 Migrations",
                    LinkUrl = "https://makandracards.com/curriculum/35281-migrations-2d",
                    EstimatedEffort = 2.0,
                    IsInactive = false,
                    SortingIndex = 28,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "34393",
                    Title = "205 Basic file uploads and image versions",
                    LinkUrl = "https://makandracards.com/curriculum/34393-basic-file-uploads-image-versions-2d",
                    EstimatedEffort = 2.0,
                    IsInactive = false,
                    SortingIndex = 29,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "43994",
                    Title = "210 Cucumber in depth",
                    LinkUrl = "https://makandracards.com/curriculum/43994-cucumber-depth-2d",
                    EstimatedEffort = 2.0,
                    IsInactive = false,
                    SortingIndex = 30,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "35615",
                    Title = "215 Browser automation with Capybara and Selenium WebDriver",
                    LinkUrl = "https://makandracards.com/curriculum/35615-browser-automation-capybara-selenium-webdriver-2d",
                    EstimatedEffort = 2.0,
                    IsInactive = false,
                    SortingIndex = 31,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "34409",
                    Title = "220 How makandra makes money",
                    LinkUrl = "https://makandracards.com/curriculum/34409-makandra-makes-money-0-5d",
                    EstimatedEffort = 0.5,
                    IsInactive = false,
                    SortingIndex = 32,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "34401",
                    Title = "224 Advanced git",
                    LinkUrl = "https://makandracards.com/curriculum/34401-advanced-git-2d",
                    EstimatedEffort = 2.0,
                    IsInactive = false,
                    SortingIndex = 33,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "35701",
                    Title = "225 Event bubbling and delegation",
                    LinkUrl = "https://makandracards.com/curriculum/35701-event-bubbling-delegation-1-5d",
                    EstimatedEffort = 1.5,
                    IsInactive = false,
                    SortingIndex = 34,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "34977",
                    Title = "230 Unobtrusive JavaScript components",
                    LinkUrl = "https://makandracards.com/curriculum/34977-unobtrusive-javascript-components-3d",
                    EstimatedEffort = 3.0,
                    IsInactive = false,
                    SortingIndex = 35,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "36041",
                    Title = "235 Cookies and Rails Sessions",
                    LinkUrl = "https://makandracards.com/curriculum/36041-cookies-rails-sessions-1d",
                    EstimatedEffort = 1.0,
                    IsInactive = false,
                    SortingIndex = 36,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "35023",
                    Title = "237 Web application security",
                    LinkUrl = "https://makandracards.com/curriculum/35023-web-application-security-4d",
                    EstimatedEffort = 4.0,
                    IsInactive = false,
                    SortingIndex = 37,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "35013",
                    Title = "240 Authentication",
                    LinkUrl = "https://makandracards.com/curriculum/35013-authentication-3d",
                    EstimatedEffort = 3.0,
                    IsInactive = false,
                    SortingIndex = 38,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "35017",
                    Title = "245 Authorization",
                    LinkUrl = "https://makandracards.com/curriculum/35017-authorization-2-5d",
                    EstimatedEffort = 2.5,
                    IsInactive = false,
                    SortingIndex = 39,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "35779",
                    Title = "247 Nested forms",
                    LinkUrl = "https://makandracards.com/curriculum/35779-nested-forms-2d",
                    EstimatedEffort = 2.0,
                    IsInactive = false,
                    SortingIndex = 40,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "44004",
                    Title = "248 Deleting associated records",
                    LinkUrl = "https://makandracards.com/curriculum/44004-deleting-associated-records-1d",
                    EstimatedEffort = 1.0,
                    IsInactive = false,
                    SortingIndex = 41,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "34985",
                    Title = "250 Form models",
                    LinkUrl = "https://makandracards.com/curriculum/34985-form-models-2d",
                    EstimatedEffort = 2.0,
                    IsInactive = false,
                    SortingIndex = 42,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "508441",
                    Title = "255 Parsing text with regular expressions",
                    LinkUrl = "https://makandracards.com/curriculum/508441-parsing-text-regular-expressions-1d",
                    EstimatedEffort = 1.0,
                    IsInactive = false,
                    SortingIndex = 43,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "35301",
                    Title = "260 Network basics",
                    LinkUrl = "https://makandracards.com/curriculum/35301-network-basics-0-5d",
                    EstimatedEffort = 0.5,
                    IsInactive = true,
                    SortingIndex = 44,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "35311",
                    Title = "265 High-availability operations",
                    LinkUrl = "https://makandracards.com/curriculum/35311-high-availability-operations-1d",
                    EstimatedEffort = 1.0,
                    IsInactive = false,
                    SortingIndex = 45,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "34975",
                    Title = "270 More Software design",
                    LinkUrl = "https://makandracards.com/curriculum/34975-software-design-2d",
                    EstimatedEffort = 2.0,
                    IsInactive = false,
                    SortingIndex = 46,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "34399",
                    Title = "275 The HTML5 platform",
                    LinkUrl = "https://makandracards.com/curriculum/34399-html5-platform-1-5d",
                    EstimatedEffort = 1.5,
                    IsInactive = false,
                    SortingIndex = 47,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "514138",
                    Title = "285 Frontend build pipelines in Rails",
                    LinkUrl = "https://makandracards.com/curriculum/514138-frontend-build-pipelines-rails-3d",
                    EstimatedEffort = 3.0,
                    IsInactive = false,
                    SortingIndex = 48,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "36037",
                    Title = "287 The asset pipeline",
                    LinkUrl = "https://makandracards.com/curriculum/36037-asset-pipeline-1d",
                    EstimatedEffort = 1.0,
                    IsInactive = true,
                    SortingIndex = 49,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "521737",
                    Title = "288 50% Milestone",
                    LinkUrl = "https://makandracards.com/curriculum/521737-50-milestone",
                    EstimatedEffort = 0,
                    IsInactive = false,
                    SortingIndex = 50,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "35675",
                    Title = "290 Structuring CSS with the BEM pattern",
                    LinkUrl = "https://makandracards.com/curriculum/35675-structuring-css-bem-pattern-4d",
                    EstimatedEffort = 4.0,
                    IsInactive = false,
                    SortingIndex = 51,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "35677",
                    Title = "292 CSS: Fluid and responsive layouts",
                    LinkUrl = "https://makandracards.com/curriculum/35677-css-fluid-responsive-layouts-1d",
                    EstimatedEffort = 1.0,
                    IsInactive = false,
                    SortingIndex = 52,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "34979",
                    Title = "295 Advanced JavaScript",
                    LinkUrl = "https://makandracards.com/curriculum/34979-advanced-javascript-2-5d",
                    EstimatedEffort = 2.5,
                    IsInactive = false,
                    SortingIndex = 53,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "35275",
                    Title = "300 JavaScript: Writing asynchronous code",
                    LinkUrl = "https://makandracards.com/curriculum/35275-javascript-writing-asynchronous-code-2-5d",
                    EstimatedEffort = 2.5,
                    IsInactive = false,
                    SortingIndex = 54,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "43998",
                    Title = "301 Using external JavaScript libraries",
                    LinkUrl = "https://makandracards.com/curriculum/43998-external-javascript-libraries-1-5d",
                    EstimatedEffort = 1.5,
                    IsInactive = false,
                    SortingIndex = 55,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "35271",
                    Title = "305 Internal APIs and client-side rendering",
                    LinkUrl = "https://makandracards.com/curriculum/35271-internal-apis-client-side-rendering",
                    EstimatedEffort = 0,
                    IsInactive = true,
                    SortingIndex = 56,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "60307",
                    Title = "310 Unpoly",
                    LinkUrl = "https://makandracards.com/curriculum/60307-unpoly-3d",
                    EstimatedEffort = 3.0,
                    IsInactive = false,
                    SortingIndex = 57,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "35019",
                    Title = "315 Advanced Ruby: Metaprogramming and DSLs",
                    LinkUrl = "https://makandracards.com/curriculum/35019-advanced-ruby-metaprogramming-dsls-3d",
                    EstimatedEffort = 3.0,
                    IsInactive = false,
                    SortingIndex = 58,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "35877",
                    Title = "316 Advanced Ruby: More metaprogramming with Modularity and ActiveSupport::Concern",
                    LinkUrl = "https://makandracards.com/curriculum/35877-advanced-ruby-metaprogramming-modularity-activesupport",
                    EstimatedEffort = 2.0,
                    IsInactive = false,
                    SortingIndex = 59,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "35681",
                    Title = "320 State machines",
                    LinkUrl = "https://makandracards.com/curriculum/35681-state-machines-3d",
                    EstimatedEffort = 3.0,
                    IsInactive = false,
                    SortingIndex = 60,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "35269",
                    Title = "325 Consuming external APIs with JavaScript",
                    LinkUrl = "https://makandracards.com/curriculum/35269-consuming-external-apis-javascript-2d",
                    EstimatedEffort = 2.0,
                    IsInactive = false,
                    SortingIndex = 61,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "43388",
                    Title = "326 Consuming external APIs with Ruby",
                    LinkUrl = "https://makandracards.com/curriculum/43388-consuming-external-apis-ruby-1d",
                    EstimatedEffort = 1.0,
                    IsInactive = false,
                    SortingIndex = 62,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "508442",
                    Title = "328 Testing JavaScript with Jasmine",
                    LinkUrl = "https://makandracards.com/curriculum/508442-testing-javascript-jasmine-1-5d",
                    EstimatedEffort = 1.5,
                    IsInactive = false,
                    SortingIndex = 63,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "35279",
                    Title = "335 Secure storage of file attachments",
                    LinkUrl = "https://makandracards.com/curriculum/35279-secure-storage-file-attachments-2d",
                    EstimatedEffort = 2.0,
                    IsInactive = false,
                    SortingIndex = 64,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "35657",
                    Title = "350 UI design basics",
                    LinkUrl = "https://makandracards.com/curriculum/35657-ui-design-basics-2-5d",
                    EstimatedEffort = 2.5,
                    IsInactive = false,
                    SortingIndex = 65,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "60301",
                    Title = "352 Implementing Design + Flexbox",
                    LinkUrl = "https://makandracards.com/curriculum/60301-implementing-design-flexbox-3-5d",
                    EstimatedEffort = 3.5,
                    IsInactive = false,
                    SortingIndex = 66,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "525354",
                    Title = "353 CSS Grids",
                    LinkUrl = "https://makandracards.com/curriculum/525354-css-grids-2d",
                    EstimatedEffort = 2.0,
                    IsInactive = false,
                    SortingIndex = 67,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "35693",
                    Title = "355 Requirements analysis and minimum viable UIs",
                    LinkUrl = "https://makandracards.com/curriculum/35693-requirements-analysis-minimum-viable-uis-3d",
                    EstimatedEffort = 3.0,
                    IsInactive = false,
                    SortingIndex = 68,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "35619",
                    Title = "365 Estimates",
                    LinkUrl = "https://makandracards.com/curriculum/35619-estimates-2-5d",
                    EstimatedEffort = 2.5,
                    IsInactive = false,
                    SortingIndex = 69,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "508440",
                    Title = "367 Your career",
                    LinkUrl = "https://makandracards.com/curriculum/508440-career-0-75d",
                    EstimatedEffort = 0.75,
                    IsInactive = false,
                    SortingIndex = 70,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "35687",
                    Title = "370 Bonus: Technology choice",
                    LinkUrl = "https://makandracards.com/curriculum/35687-bonus-technology-choice-0-5d",
                    EstimatedEffort = 0.5,
                    IsInactive = false,
                    SortingIndex = 71,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "43386",
                    Title = "372 Dealing with legacy applications",
                    LinkUrl = "https://makandracards.com/curriculum/43386-dealing-legacy-applications-0-75d",
                    EstimatedEffort = 0.75,
                    IsInactive = false,
                    SortingIndex = 72,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "34991",
                    Title = "385 Rack and Middlewares",
                    LinkUrl = "https://makandracards.com/curriculum/34991-rack-middlewares",
                    EstimatedEffort = 0,
                    IsInactive = true,
                    SortingIndex = 73,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "36021",
                    Title = "395 Background processing",
                    LinkUrl = "https://makandracards.com/curriculum/36021-background-processing-2d",
                    EstimatedEffort = 2.0,
                    IsInactive = false,
                    SortingIndex = 74,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "41770",
                    Title = "396 Internationalization (I18n)",
                    LinkUrl = "https://makandracards.com/curriculum/41770-internationalization-i18n-2d",
                    EstimatedEffort = 2.0,
                    IsInactive = false,
                    SortingIndex = 75,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "34395",
                    Title = "397 Rails: Sending e-mail",
                    LinkUrl = "https://makandracards.com/curriculum/34395-rails-sending-e-mail-2d",
                    EstimatedEffort = 2.0,
                    IsInactive = false,
                    SortingIndex = 76,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "43999",
                    Title = "400 Bonus: Buzzwords and staying up to date",
                    LinkUrl = "https://makandracards.com/curriculum/43999-bonus-buzzwords-staying-date-1d",
                    EstimatedEffort = 1.0,
                    IsInactive = false,
                    SortingIndex = 77,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "35621",
                    Title = "800 AngularJS",
                    LinkUrl = "https://makandracards.com/curriculum/35621-angularjs",
                    EstimatedEffort = 0,
                    IsInactive = true,
                    SortingIndex = 78,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "35313",
                    Title = "910 Bonus: Rake",
                    LinkUrl = "https://makandracards.com/curriculum/35313-bonus-rake-0-75d",
                    EstimatedEffort = 0.75,
                    IsInactive = false,
                    SortingIndex = 79,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "35625",
                    Title = "940 Bonus: Persisting trees",
                    LinkUrl = "https://makandracards.com/curriculum/35625-bonus-persisting-trees-2d",
                    EstimatedEffort = 2.0,
                    IsInactive = false,
                    SortingIndex = 80,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "588503",
                    Title = "950 Our Project Structure",
                    LinkUrl = "https://makandracards.com/curriculum/588503-project-structure-0-5d",
                    EstimatedEffort = 0.5,
                    IsInactive = false,
                    SortingIndex = 81,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "35703",
                    Title = "960 Bonus: Managing agile projects",
                    LinkUrl = "https://makandracards.com/curriculum/35703-bonus-managing-agile-projects-1-5d",
                    EstimatedEffort = 1.5,
                    IsInactive = false,
                    SortingIndex = 82,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "621226",
                    Title = "965 Bonus: Writing documentation",
                    LinkUrl = "https://makandracards.com/curriculum/621226-bonus-writing-documentation",
                    EstimatedEffort = 0,
                    IsInactive = false,
                    SortingIndex = 83,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "34403",
                    Title = "980 Bootstrap",
                    LinkUrl = "https://makandracards.com/curriculum/34403-bootstrap-3d",
                    EstimatedEffort = 3.0,
                    IsInactive = false,
                    SortingIndex = 84,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "521453",
                    Title = "985 Bonus: Typescript",
                    LinkUrl = "https://makandracards.com/curriculum/521453-bonus-typescript",
                    EstimatedEffort = 0,
                    IsInactive = false,
                    SortingIndex = 85,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "60308",
                    Title = "985 Modern build pipelines with Webpack",
                    LinkUrl = "https://makandracards.com/curriculum/60308-modern-build-pipelines-webpack-3d",
                    EstimatedEffort = 3.0,
                    IsInactive = true,
                    SortingIndex = 86,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "60309",
                    Title = "990 Simple Form",
                    LinkUrl = "https://makandracards.com/curriculum/60309-simple-form-2d",
                    EstimatedEffort = 2.0,
                    IsInactive = false,
                    SortingIndex = 87,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "35679",
                    Title = "990 Static site generators",
                    LinkUrl = "https://makandracards.com/curriculum/35679-static-site-generators",
                    EstimatedEffort = 0,
                    IsInactive = true,
                    SortingIndex = 88,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "508443",
                    Title = "992 jQuery",
                    LinkUrl = "https://makandracards.com/curriculum/508443-jquery-1-5d",
                    EstimatedEffort = 1.5,
                    IsInactive = true,
                    SortingIndex = 89,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "508444",
                    Title = "994 CoffeeScript",
                    LinkUrl = "https://makandracards.com/curriculum/508444-coffeescript-0-5d",
                    EstimatedEffort = 0.5,
                    IsInactive = true,
                    SortingIndex = 90,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "35683",
                    Title = "995 Bonus: Images",
                    LinkUrl = "https://makandracards.com/curriculum/35683-bonus-images-1-5d",
                    EstimatedEffort = 1.5,
                    IsInactive = false,
                    SortingIndex = 91,
                    TeachingPlanId = 3
                },

                new Lesson {
                    MakandraId = "559084",
                    Title = "1000 100% Milestone",
                    LinkUrl = "https://makandracards.com/curriculum/559084-100-milestone",
                    EstimatedEffort = 0,
                    IsInactive = false,
                    SortingIndex = 92,
                    TeachingPlanId = 3
                }
            };

            lessons.AddRange(lessons2);

            // ++++++++++++++++
            foreach (var lesson in lessons) {
                bool alreadyExists = await _databaseLessonRepository
                    .ExistsAsync(lesson.MakandraId, lesson.TeachingPlanId);

                if (!alreadyExists) {
                    await _databaseLessonRepository.CreateAsync(lesson);
                }
            }
        }

        // ---------------------------------------------------
        /// <summary>
        /// Seeds predefined teaching plans (WebDevelopment, DevOps, WebDevJSON) 
        /// and links them with corresponding lessons.
        /// </summary>
        public async Task SeedTeachingPlansAsync() {
            var allLessons = (await _databaseLessonRepository.GetAllLessonsAsync()).ToList();

            var plans = new[] {
                new {
                    Id = 1,
                    Name = "WebDevelopment"
                },
                new {
                    Id = 2,
                    Name = "DevOps"
                },
                new {
                    Id = 3,
                    Name = "WebDevJSON"
                }
            };


            foreach (var plan in plans) {
                var existing = await _databaseTeachingPlanRepository.GetTeachingPlanByIdWithLessonsAndTraineesAsync(plan.Id);

                // Only add teachingPlan if not existing yet
                if (existing != null)
                    continue;

                var lessons = allLessons
                    .Where(l => l.TeachingPlanId == plan.Id)
                    .ToList();

                var teachingPlan = new TeachingPlan {
                    TeachingPlanId = plan.Id,
                    Name = plan.Name,
                    LastUpdated = DateTime.UtcNow,
                };

                await _databaseTeachingPlanRepository.CreateAsync(teachingPlan);
            }
        }

        // ---------------------------------------------------
        /// <summary>
        /// Seeds predefined users (admins, mentors, trainees) into the system using the AdminService.
        /// Closes the user 'closed.traineeTEST@uni-a.de' if seeded.
        /// </summary>
        public async Task SeedUsersAsync() {

            var userData = new List<ApplicationUserDto> {

                // ++++++++++++++++
                // Admin
                new ApplicationUserDto {
                    Email = "simon.hinterreiter@uni-a.de",
                    Role = "Admin",
                    Password = "Sopro.2025"
                },

                // Endabnahme - 01
                new ApplicationUserDto {
                    Email = "admin@makandra.de",
                    Role = "Admin",
                    Password = "Admin1!"
                },

                // ++++++++++++++++
                // Mentor
                new ApplicationUserDto {
                    Email = "paul.schweizerTEST@uni-a.de",
                    Role = "Mentor",
                    Password = "Sopro.2025"
                },


                // Endabnahme - 05
                new ApplicationUserDto {
                    Email = "manfred.mental@makandra.de",
                    Role = "Mentor",
                    Password = "Pssssst1!"
                },

                // Endabnahme - 06
                new ApplicationUserDto {
                    Email = "hans.hilfreich@makandra.de",
                    Role = "Mentor",
                    Password = "Hilfe123!"
                },

                // ++++++++++++++++
                // Trainee
                new ApplicationUserDto {
                    Email = "alexander.schlemmerTEST@uni-a.de",
                    Role = "Trainee",
                    TeachingPlanId = 2, // DevOps
                    Password = "Sopro.2025",
                    TraineeStartDate = new DateOnly(2025, 4, 1),
                    TraineeEndDate = new DateOnly(2026, 7, 1)
                },

                new ApplicationUserDto {
                    Email = "alexandros.blaskTEST@uni-a.de",
                    Role = "Trainee",
                    TeachingPlanId = 1, // WebDev
                    Password = "Sopro.2025",
                    TraineeStartDate = new DateOnly(2025, 4, 1),
                    TraineeEndDate = new DateOnly(2026, 7, 1)
                },

                new ApplicationUserDto {
                    Email = "nikita.stefanTEST@uni-a.de",
                    Role = "Trainee",
                    TeachingPlanId = 2, // DevOps
                    Password = "Sopro.2025",
                    TraineeStartDate = new DateOnly(2025, 3, 15),
                    TraineeEndDate = new DateOnly(2026, 6, 30)
                },

                new ApplicationUserDto {
                    Email = "closed.traineeTEST@uni-a.de",
                    Role = "Trainee",
                    TeachingPlanId = 2, // DevOps
                    Password = "Sopro.2025",
                    TraineeStartDate = new DateOnly(2025, 3, 15),
                    TraineeEndDate = new DateOnly(2026, 6, 30)
                },

                // Endabnahme - 02
                new ApplicationUserDto {
                    Email = "vanessa.vital@makandra.de",
                    Role = "Trainee",
                    TeachingPlanId = 3, //WebDevJSON
                    Password = "VVital13!",
                    TraineeStartDate = new DateOnly(2025, 08, 01),
                    TraineeEndDate = new DateOnly(2026, 03, 31)
                },

                // Endabnahme - 03
                new ApplicationUserDto {
                    Email = "stefan.schnupfen@makandra.de",
                    Role = "Trainee",
                    TeachingPlanId = 3, //WebDevJSON
                    Password = "Stefan1!",
                    TraineeStartDate = new DateOnly(2025, 05, 01),
                    TraineeEndDate = new DateOnly(2025, 10, 31)
                },

                // Endabnahme - 04
                new ApplicationUserDto {
                    Email = "ursula.urlaub@makandra.de",
                    Role = "Trainee",
                    TeachingPlanId = 3, //WebDevJSON
                    Password = "U1laub!",
                    TraineeStartDate = new DateOnly(2025, 01, 01),
                    TraineeEndDate = new DateOnly(2025, 08, 31)
                },

                // ++++++++++++++++
            };

            foreach (var dto in userData) {
                await _adminService.CreateUserAsync(dto, true);

                if (dto.Email == "closed.traineeTEST@uni-a.de") {
                    var user = await _databaseApplicationUserRepository.FindByEmailAsync(dto.Email);
                    if (user != null) {
                        await _adminService.CloseUserAsync(user.Id);
                    }
                }
            }
        }

        // ---------------------------------------------------
        /// <summary>
        /// Seeds predefined processing pauses for selected trainees 
        /// (alex, nikita, stefan, ursula) based on fixed date intervals.
        /// </summary>
        public async Task SeedProcessingPausesAsync() {
            var alex = await _databaseApplicationUserRepository.FindByEmailAsync("alexandros.blaskTEST@uni-a.de");
            var nikita = await _databaseApplicationUserRepository.FindByEmailAsync("nikita.stefanTEST@uni-a.de");
            var stefan = await _databaseApplicationUserRepository.FindByEmailAsync("stefan.schnupfen@makandra.de");
            var ursula = await _databaseApplicationUserRepository.FindByEmailAsync("ursula.urlaub@makandra.de");

            var pauseDtos = new List<ProcessingPauseDto>();

            if (alex != null) {
                pauseDtos.Add(new ProcessingPauseDto {
                    TraineeId = alex.Id,
                    StartDate = new DateOnly(2025, 5, 5),
                    EndDate = new DateOnly(2025, 5, 10)
                });

                pauseDtos.Add(new ProcessingPauseDto {
                    TraineeId = alex.Id,
                    StartDate = new DateOnly(2025, 6, 1),
                    EndDate = new DateOnly(2025, 6, 3)
                });
            }

            if (nikita != null) {
                pauseDtos.Add(new ProcessingPauseDto {
                    TraineeId = nikita.Id,
                    StartDate = new DateOnly(2025, 5, 20),
                    EndDate = new DateOnly(2025, 5, 25)
                });
            }
            if (stefan != null) {
                pauseDtos.Add(new ProcessingPauseDto {
                    TraineeId = stefan.Id,
                    StartDate = new DateOnly(2025, 5, 20),
                    EndDate = new DateOnly(2025, 5, 25)
                });
            }
            if (ursula != null) {
                pauseDtos.Add(new ProcessingPauseDto {
                    TraineeId = ursula.Id,
                    StartDate = new DateOnly(2025, 5, 20),
                    EndDate = new DateOnly(2025, 5, 25)
                });
            }
            foreach (var dto in pauseDtos) {
                await _adminService.CreateProcessingPauseAsync(dto);
            }

        }

        // ---------------------------------------------------
        /// <summary>
        /// Seeds feedback entries for selected trainees on specific lessons, 
        /// including ratings, effort, and difficulty, and assigns one reader (mentor/admin).
        /// Also sets lesson state to "Rated" and adds a corresponding lesson log entry.
        /// </summary>
        public async Task SeedFeedbackAsync() {

            // Get Trainees
            var alex = await _databaseApplicationUserRepository.FindByEmailAsync("alexandros.blaskTEST@uni-a.de");
            var nikita = await _databaseApplicationUserRepository.FindByEmailAsync("nikita.stefanTEST@uni-a.de");
            var schlemmer = await _databaseApplicationUserRepository.FindByEmailAsync("alexander.schlemmerTEST@uni-a.de");
            // Get Mentor and Admin who read it
            var simon = await _databaseApplicationUserRepository.FindByEmailAsync("simon.hinterreiter@uni-a.de"); // Admin
            var paul = await _databaseApplicationUserRepository.FindByEmailAsync("paul.schweizerTEST@uni-a.de");     // Mentor

            // FeedbackSeeds
            var feedbackSeeds = new List<(ApplicationUser? user, Lesson? lesson, LessonDifficulty difficulty, float effort, PreviousKnowledgeLevel previousKnowledge, string? comment)> {

                // Alex - WebDev
                (alex, await _databaseLessonRepository.GetLessonByIdAsync(2), LessonDifficulty.VeryEasy, 2.2f, PreviousKnowledgeLevel.Advanced, "Clear material, just a bit verbose."),
                (alex, await _databaseLessonRepository.GetLessonByIdAsync(3), LessonDifficulty.Easy, 2.0f, PreviousKnowledgeLevel.Basic, "Good structure, helpful hands-on tasks."),
                (alex, await _databaseLessonRepository.GetLessonByIdAsync(4), LessonDifficulty.Hard, 2.5f, PreviousKnowledgeLevel.Intermediate, "Challenging parts popped up unexpectedly."),
                (alex, await _databaseLessonRepository.GetLessonByIdAsync(5), LessonDifficulty.Medium, 2.2f, PreviousKnowledgeLevel.Intermediate, "Enjoyed the progression, though a bit long."),
                (alex, await _databaseLessonRepository.GetLessonByIdAsync(6), LessonDifficulty.VeryEasy, 2.8f, PreviousKnowledgeLevel.Expert, "Still managed to learn some new tricks."),
                (alex, await _databaseLessonRepository.GetLessonByIdAsync(7), LessonDifficulty.Medium, 1.8f, PreviousKnowledgeLevel.Basic, "Quick to implement, examples were useful."),
                (alex, await _databaseLessonRepository.GetLessonByIdAsync(8), LessonDifficulty.Easy, 2.6f, PreviousKnowledgeLevel.Aware, "Tutorials helped clarify key ideas."),
                (alex, await _databaseLessonRepository.GetLessonByIdAsync(9), LessonDifficulty.VeryEasy, 2.8f, PreviousKnowledgeLevel.Advanced, "Took some time but solidified fundamentals."),
                (alex, await _databaseLessonRepository.GetLessonByIdAsync(10), LessonDifficulty.VeryHard, 2.6f, PreviousKnowledgeLevel.Basic, "Needed to look things up, but manageable."),
                (alex, await _databaseLessonRepository.GetLessonByIdAsync(11), LessonDifficulty.Medium, 2.3f, PreviousKnowledgeLevel.Basic, "Exercises made the lesson click."),
                (alex, await _databaseLessonRepository.GetLessonByIdAsync(12), LessonDifficulty.VeryHard, 2.9f, PreviousKnowledgeLevel.Intermediate, "Ran into minor issues, but figured it out."),
                (alex, await _databaseLessonRepository.GetLessonByIdAsync(13), LessonDifficulty.Easy, 2.1f, PreviousKnowledgeLevel.Advanced, "Mostly reinforcing things I already knew."),
                (alex, await _databaseLessonRepository.GetLessonByIdAsync(14), LessonDifficulty.Easy, 1.4f, PreviousKnowledgeLevel.Intermediate, "Breezed through most of it."),
                (alex, await _databaseLessonRepository.GetLessonByIdAsync(15), LessonDifficulty.VeryHard, 2.0f, PreviousKnowledgeLevel.Basic, "Good recap, not too deep."),
                (alex, await _databaseLessonRepository.GetLessonByIdAsync(16), LessonDifficulty.Easy, 2.9f, PreviousKnowledgeLevel.Intermediate, "Took longer than expected but worth it."),
                (alex, await _databaseLessonRepository.GetLessonByIdAsync(17), LessonDifficulty.Hard, 1.6f, PreviousKnowledgeLevel.Advanced, "Smooth process, clear goals."),
                (alex, await _databaseLessonRepository.GetLessonByIdAsync(18), LessonDifficulty.VeryEasy, 1.7f, PreviousKnowledgeLevel.Advanced, "Was already comfortable with most tasks."),
                (alex, await _databaseLessonRepository.GetLessonByIdAsync(19), LessonDifficulty.VeryHard, 2.1f, PreviousKnowledgeLevel.Aware, "Could’ve skipped a few parts, but still helpful."),
                (alex, await _databaseLessonRepository.GetLessonByIdAsync(20), LessonDifficulty.Medium, 1.6f, PreviousKnowledgeLevel.Basic, "No surprises here – steady pace."),

                // Stefan - DevOps
                (nikita, await _databaseLessonRepository.GetLessonByIdAsync(41), LessonDifficulty.VeryEasy, 2.5f, PreviousKnowledgeLevel.Advanced, "Most of the content was familiar, but the structure helped reinforce it."),
                (nikita, await _databaseLessonRepository.GetLessonByIdAsync(42), LessonDifficulty.VeryHard, 2.4f, PreviousKnowledgeLevel.Aware, "Nice and easy to go through, not much new for me."),
                (nikita, await _databaseLessonRepository.GetLessonByIdAsync(43), LessonDifficulty.Easy, 2.6f, PreviousKnowledgeLevel.Advanced, "I remembered a lot as I worked through it, still useful."),
                (nikita, await _databaseLessonRepository.GetLessonByIdAsync(44), LessonDifficulty.Hard, 2.1f, PreviousKnowledgeLevel.Intermediate, "It went quickly because I had done similar tasks before."),
                (nikita, await _databaseLessonRepository.GetLessonByIdAsync(45), LessonDifficulty.VeryHard, 2.0f, PreviousKnowledgeLevel.Expert, "Skimmed most of it, only reviewed one new concept."),
                (nikita, await _databaseLessonRepository.GetLessonByIdAsync(46), LessonDifficulty.VeryHard, 2.1f, PreviousKnowledgeLevel.Advanced, "Just revisited a few key ideas, rest was known."),
                (nikita, await _databaseLessonRepository.GetLessonByIdAsync(47), LessonDifficulty.VeryHard, 2.2f, PreviousKnowledgeLevel.Intermediate, "Efficient to work through, good confidence boost."),
                (nikita, await _databaseLessonRepository.GetLessonByIdAsync(48), LessonDifficulty.Medium, 2.5f, PreviousKnowledgeLevel.Basic, "Felt familiar, but a few details were nice to recall."),
                (nikita, await _databaseLessonRepository.GetLessonByIdAsync(49), LessonDifficulty.VeryHard, 2.2f, PreviousKnowledgeLevel.Aware, "I just brushed up on the terms."),
                (nikita, await _databaseLessonRepository.GetLessonByIdAsync(50), LessonDifficulty.VeryEasy, 1.3f, PreviousKnowledgeLevel.Expert, "Breezed through – I could do this lesson blind."),
                (nikita, await _databaseLessonRepository.GetLessonByIdAsync(51), LessonDifficulty.Easy, 2.8f, PreviousKnowledgeLevel.Intermediate, "No surprises, but well presented."),
                (nikita, await _databaseLessonRepository.GetLessonByIdAsync(52), LessonDifficulty.Easy, 1.4f, PreviousKnowledgeLevel.Intermediate, "Quick and efficient to complete."),
                (nikita, await _databaseLessonRepository.GetLessonByIdAsync(53), LessonDifficulty.VeryHard, 1.8f, PreviousKnowledgeLevel.Advanced, "I used it more as a self-check."),
                (nikita, await _databaseLessonRepository.GetLessonByIdAsync(54), LessonDifficulty.VeryEasy, 2.0f, PreviousKnowledgeLevel.Advanced, "I remembered most from previous projects."),
                (nikita, await _databaseLessonRepository.GetLessonByIdAsync(55), LessonDifficulty.Medium, 2.5f, PreviousKnowledgeLevel.Intermediate, "I went through it smoothly, good review."),
                (nikita, await _databaseLessonRepository.GetLessonByIdAsync(56), LessonDifficulty.VeryHard, 2.3f, PreviousKnowledgeLevel.Basic, "Just needed to remind myself of a few terms."),
                (nikita, await _databaseLessonRepository.GetLessonByIdAsync(57), LessonDifficulty.Easy, 2.1f, PreviousKnowledgeLevel.Intermediate, "The structure helped reinforce a few edge cases."),
                (nikita, await _databaseLessonRepository.GetLessonByIdAsync(58), LessonDifficulty.VeryEasy, 2.2f, PreviousKnowledgeLevel.Expert, "I’ve done this several times already."),
                (nikita, await _databaseLessonRepository.GetLessonByIdAsync(59), LessonDifficulty.Easy, 1.5f, PreviousKnowledgeLevel.Advanced, "Didn’t take much time – was confident going in."),
                (nikita, await _databaseLessonRepository.GetLessonByIdAsync(60), LessonDifficulty.Medium, 2.1f, PreviousKnowledgeLevel.Intermediate, "Some overlap with earlier"),

                // Ursula - DevOps
                (schlemmer, await _databaseLessonRepository.GetLessonByIdAsync(41), LessonDifficulty.Hard, 2.8f, PreviousKnowledgeLevel.Intermediate, "It required careful reading, but I got through it."),
                (schlemmer, await _databaseLessonRepository.GetLessonByIdAsync(42), LessonDifficulty.Medium, 3.1f, PreviousKnowledgeLevel.Basic, "A good refresher with some deeper insights."),
                (schlemmer, await _databaseLessonRepository.GetLessonByIdAsync(43), LessonDifficulty.Medium, 2.5f, PreviousKnowledgeLevel.Intermediate, "Easy to follow and very practical."),
                (schlemmer, await _databaseLessonRepository.GetLessonByIdAsync(44), LessonDifficulty.VeryHard, 2.0f, PreviousKnowledgeLevel.Basic, "I appreciated the examples throughout."),
                (schlemmer, await _databaseLessonRepository.GetLessonByIdAsync(45), LessonDifficulty.Hard, 2.7f, PreviousKnowledgeLevel.Intermediate, "Not too hard, not too easy."),
                (schlemmer, await _databaseLessonRepository.GetLessonByIdAsync(46), LessonDifficulty.Medium, 3.0f, PreviousKnowledgeLevel.Intermediate, "Some tricky parts, but doable."),
                (schlemmer, await _databaseLessonRepository.GetLessonByIdAsync(47), LessonDifficulty.Insane, 1.8f, PreviousKnowledgeLevel.Aware, "A smooth experience overall."),
                (schlemmer, await _databaseLessonRepository.GetLessonByIdAsync(48), LessonDifficulty.Medium, 2.9f, PreviousKnowledgeLevel.Intermediate, "The tasks matched the theory well."),
                (schlemmer, await _databaseLessonRepository.GetLessonByIdAsync(49), LessonDifficulty.Medium, 2.3f, PreviousKnowledgeLevel.Intermediate, "I liked how it built on earlier lessons."),
                (schlemmer, await _databaseLessonRepository.GetLessonByIdAsync(50), LessonDifficulty.Hard, 3.2f, PreviousKnowledgeLevel.Advanced, "I had to revisit parts to fully understand."),
                (schlemmer, await _databaseLessonRepository.GetLessonByIdAsync(51), LessonDifficulty.VeryHard, 3.5f, PreviousKnowledgeLevel.Advanced, "I learned a lot from this one."),
                (schlemmer, await _databaseLessonRepository.GetLessonByIdAsync(52), LessonDifficulty.VeryHard, 2.1f, PreviousKnowledgeLevel.Basic, "Mostly review, but still useful."),
                (schlemmer, await _databaseLessonRepository.GetLessonByIdAsync(53), LessonDifficulty.Hard, 2.6f, PreviousKnowledgeLevel.Intermediate, "Good level of depth for a single lesson."),
                (schlemmer, await _databaseLessonRepository.GetLessonByIdAsync(54), LessonDifficulty.Medium, 3.0f, PreviousKnowledgeLevel.Intermediate, "I enjoyed working through it."),
                (schlemmer, await _databaseLessonRepository.GetLessonByIdAsync(55), LessonDifficulty.Medium, 2.7f, PreviousKnowledgeLevel.Intermediate, "Real-world relevance made it easier."),
                (schlemmer, await _databaseLessonRepository.GetLessonByIdAsync(56), LessonDifficulty.Hard, 2.4f, PreviousKnowledgeLevel.Intermediate, "Challenging enough to stay focused."),
                (schlemmer, await _databaseLessonRepository.GetLessonByIdAsync(57), LessonDifficulty.Medium, 2.8f, PreviousKnowledgeLevel.Intermediate, "The exercises helped solidify things."),
                (schlemmer, await _databaseLessonRepository.GetLessonByIdAsync(58), LessonDifficulty.Medium, 3.1f, PreviousKnowledgeLevel.Intermediate, "I had to look up a few things."),
                (schlemmer, await _databaseLessonRepository.GetLessonByIdAsync(59), LessonDifficulty.Hard, 3.3f, PreviousKnowledgeLevel.Advanced, "Took longer than expected."),
                (schlemmer, await _databaseLessonRepository.GetLessonByIdAsync(60), LessonDifficulty.Medium, 2.5f, PreviousKnowledgeLevel.Intermediate, "Felt productive afterward.")
            };

            var readers = new List<ApplicationUser?> { simon, paul };
            int readerIndex = 0;

            foreach (var (user, lesson, difficulty, effort, previousKnowledge, comment) in feedbackSeeds) {
                if (user == null || lesson == null)
                    continue;

                var reader = readers[readerIndex % readers.Count];
                readerIndex++;

                if (reader == null)
                    continue;

                // Get the correct TraineeLesson
                var traineeLessons = await _databaseTraineeLessonRepository.GetAllTraineeLessonsOfTraineeWithLessonAsync(user.Id);
                var traineeLesson = traineeLessons.FirstOrDefault(tl => tl.Lesson.LessonId == lesson.LessonId);
                if (traineeLesson == null)
                    continue;


                // Change the State to Rated
                traineeLesson.State = TraineeLessonState.Rated;
                await _databaseTraineeLessonRepository.UpdateAsync(traineeLesson);

                // Check if Feedback already exists
                var existingFeedbacks = await _databaseFeedbackRepository
                    .GetAllFeedbacksWrittenByUserWithLessonAndAuthorAndReadByUsersAsync(user);

                if (existingFeedbacks.Any(f => f.LessonId == lesson.LessonId))
                    continue;

                // Create Feedback Objekt
                var feedback = new Feedback {
                    Difficulty = difficulty,
                    HoursOfEffort = effort,
                    PreviousKnowledge = previousKnowledge,
                    Comment = comment,
                    LessonId = lesson.LessonId,
                    Lesson = lesson,
                    AuthorId = user.Id,
                    Author = user,
                    ReadByUsers = new List<ApplicationUser> { reader }
                };

                // Store in DB
                await _databaseFeedbackRepository.CreateAsync(feedback);


                var logEntry = new TraineeLessonLogEntry {
                    TraineeLessonId = traineeLesson.TraineeLessonId,
                    LessonName = lesson.Title,
                    UserId = user.Id,
                    UserName = user.Email!,
                    OldState = "Accepted",
                    NewState = "Rated",
                    Timestamp = DateTime.UtcNow
                };

                _databaseTraineeLessonLogEntryRepository.Create(logEntry);
            }
        }

        // ---------------------------------------------------
        /// <summary>
        /// Seeds lesson progress by marking a percentage of lessons as "Finished"
        /// for Stefan and Ursula. Uses <see cref="SeedProgressAsync"/>.
        /// </summary>
        public async Task SeedProgressForStefanAndUrsulaAsync() {
            var stefan = await _databaseApplicationUserRepository
                .FindByEmailWithProcessingPausesAndTraineeLessonsAsync("stefan.schnupfen@makandra.de");

            var ursula = await _databaseApplicationUserRepository
                .FindByEmailWithProcessingPausesAndTraineeLessonsAsync("ursula.urlaub@makandra.de");

            if (stefan?.TeachingPlanId == null || ursula?.TeachingPlanId == null)
                throw new Exception("Stefan or Ursula have no TeachingPlan.");

            await SeedProgressAsync(stefan, 30);
            await SeedProgressAsync(ursula, 90);
        }

        // ---------------------------------------------------
        /// <summary>
        /// Marks a specified percentage of active lessons in a trainee's plan as "Finished".
        /// </summary>
        /// <param name="trainee">The trainee whose progress should be updated.</param>
        /// <param name="percentage">The percentage of lessons to mark as finished.</param>
        private async Task SeedProgressAsync(ApplicationUser trainee, int percentage) {
            var existingLessons = trainee.TraineeLessons
                .Where(tl => !tl.Lesson.IsInactive)
                .OrderBy(tl => tl.Lesson.SortingIndex)
                .ToList();

            int countToSet = (int)Math.Round(existingLessons.Count * (percentage / 100.0));

            for (int i = 0; i < countToSet; i++) {
                var tl = existingLessons[i];
                tl.State = TraineeLessonState.Finished;
                await _databaseTraineeLessonRepository.UpdateAsync(tl);
            }

            await _databaseApplicationUserRepository.UpdateAsync(trainee);
        }

        // ---------------------------------------------------
    }
}