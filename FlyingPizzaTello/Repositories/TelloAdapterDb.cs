using FlyingPizzaTello.Entities;
using Microsoft.EntityFrameworkCore;

namespace FlyingPizzaTello.Repositories;

public class TelloAdapterDb : DbContext
{
    public TelloAdapterDb(DbContextOptions<TelloAdapterDb> options) : base(options){}
    public DbSet<TelloAdapter> TelloAdapters => Set<TelloAdapter>();
    
}