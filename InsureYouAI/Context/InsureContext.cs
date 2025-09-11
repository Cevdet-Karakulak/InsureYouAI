using InsureYouAI.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InsureYouAI.Context;

public class InsureContext : IdentityDbContext<AppUser>
{
    override protected void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=FLOPPA\\SQLEXPRESS01;initial Catalog=InsureDb;integrated Security=True; trust server certificate = true");
    }

    public DbSet<About> Abouts { get; set; }
    public DbSet<AboutItem> AboutItems { get; set; }
    public DbSet<Article> Articles { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Contact> Contacts { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<PricingPlan> PricingPlans { get; set; }
    public DbSet<Service> Services { get; set; }
    public DbSet<Slider> Sliders { get; set; }
    public DbSet<Testimonial> Testimonials { get; set; }
    public DbSet<TrailerVideo> TrailerVideos { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<PricingPlanItem> PricingPlanItems { get; set; }
    public DbSet<Gallery> Galleries { get; set; }
    public DbSet<ClaudeAIMessage> ClaudeAIMessages { get; set; }

}