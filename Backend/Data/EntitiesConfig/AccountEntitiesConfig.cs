using DoAnTotNghiep.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DoAnTotNghiep.Data.EntitiesConfig
{
    public class AccountEntitiesConfig : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.ToTable("Account");

            builder.HasKey(entity => entity.Id);

            builder.Property(entity => entity.UserName).HasColumnType("VARCHAR(255)").IsRequired(true);

            builder.Property(entity => entity.Password).HasColumnType("VARCHAR(255)").IsRequired(true);

            builder.Property(entity => entity.Email).HasColumnType("VARCHAR(255)").IsRequired(true);

            builder.Property(entity => entity.Phone).HasColumnType("VARCHAR(255)").IsRequired(true);

            builder.Property(entity => entity.Age).HasColumnType("int").IsRequired(true);
        }
    }
}
