using Microsoft.EntityFrameworkCore;
using StudentAPI.Models;

namespace StudentAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();

        public DbSet<StudentForm> StudentForms => Set<StudentForm>();

        public DbSet<ChatConversation> ChatConversations { get; set; }

        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<ChatSetting> ChatSettings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Student -> Conversation
            modelBuilder.Entity<ChatConversation>()
                .HasOne(c => c.Student)
                .WithMany(u => u.StudentConversations)
                .HasForeignKey(c => c.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Admin -> Conversation
            modelBuilder.Entity<ChatConversation>()
                .HasOne(c => c.Admin)
                .WithMany(u => u.AdminConversations)
                .HasForeignKey(c => c.AdminId)
                .OnDelete(DeleteBehavior.Restrict);

            // Conversation -> Messages
            modelBuilder.Entity<ChatMessage>()
                .HasOne(m => m.Conversation)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);

            // Sender -> Messages
            modelBuilder.Entity<ChatMessage>()
                .HasOne(m => m.Sender)
                .WithMany(u => u.ChatMessages)
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}