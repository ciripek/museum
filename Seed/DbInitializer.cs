using Bogus;
using Microsoft.AspNetCore.Identity;
using museum.Data;
using museum.Models;

namespace museum.Seed;

internal class DbInitializer
{
    internal static void Initialize(ApplicationDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(dbContext, nameof(dbContext));
        dbContext.Database.EnsureCreated();

        if (dbContext.Users.Any()) return;


        var items = new Faker<Item>()
            .RuleFor(o => o.Name, f => f.Random.Word())
            .RuleFor(o => o.Description, f => f.Lorem.Text())
            .RuleFor(o => o.Obtained, f => f.Date.Recent(1000))
            .RuleFor(o => o.Image, faker => faker.Image.PicsumUrl())
            .RuleFor(o => o.Labels, () => new List<Label>())
            .RuleFor(o => o.Comments, () => new List<Comment>())
            .Generate(500);


        var labels = new Faker<Label>()
            .RuleFor(label => label.Name, f => f.Random.Word())
            .RuleFor(label => label.Color, faker => faker.Internet.Color())
            .RuleFor(label => label.Display, faker => faker.Random.Bool())
            .RuleFor(label => label.Items, () => new List<Item>())
            .Generate(500);


        var random = new Random();

        foreach (var item in items)
            for (var i = 0; i < random.Next(labels.Count); i++)
                item.Labels?.Add(labels[random.Next(labels.Count)]);

        foreach (var label in labels)
            for (var i = 0; i < random.Next(items.Count); i++)
                label.Items?.Add(items[random.Next(items.Count)]);


        var comments = new Faker<Comment>()
            .RuleFor(comment => comment.Text, f => f.Lorem.Text())
            .Generate(2500);

        var userid = 1;
        var users = new Faker<ApplicationUser>()
            .RuleFor(user => user.IsAdmin, () => false)
            .RuleFor(user => user.Email, () => $"user{userid++}@szerveroldali.hu")
            .RuleFor(user => user.NormalizedEmail, () => $"user{userid++}@szerveroldali.hu".ToUpper())
            .RuleFor(user => user.UserName, () => $"user{userid++}@szerveroldali.hu")
            .RuleFor(user => user.NormalizedUserName, () => $"user{userid++}@szerveroldali.hu".ToUpper())
            .RuleFor(user => user.Comments, () => new List<Comment>())
            .Generate(50);

        var admin = new ApplicationUser
        {
            IsAdmin = true,
            Email = "admin@szerveroldali.hu",
            UserName = "admin@szerveroldali.hu",
            NormalizedEmail = "admin@szerveroldali.hu".ToUpper(),
            NormalizedUserName = "admin@szerveroldali.hu".ToUpper()
        };

        admin.PasswordHash = new PasswordHasher<ApplicationUser>().HashPassword(admin, "Alma#2001");
        users.Add(admin);

        foreach (var comment in comments)
        {
            var user = users[random.Next(users.Count)];
            var item = items[random.Next(items.Count)];

            comment.Item = item;
            comment.ApplicationUser = user;

            user.Comments?.Add(comment);
            item.Comments?.Add(comment);
        }

        dbContext.Item.AddRange(items);
        dbContext.Label.AddRange(labels);
        dbContext.Users.AddRange(users);
        dbContext.Comment.AddRange(comments);
        dbContext.SaveChanges();
    }
}