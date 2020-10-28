using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PaymentGateway.Library.Models;

namespace PaymentGateway.Data
{
    public class IdempotencyKeyRepository : IDbRespository<IdempotencyKey>
    {
        private readonly PaymentsContext _context;
        public IdempotencyKeyRepository(PaymentsContext context)
        {
            _context = context;
        }
        public virtual async Task AddNewItem(IdempotencyKey item)
        {
            await _context.IdempotencyKeys.AddAsync(item);
            await _context.SaveChangesAsync();
        }

        public virtual async Task<IdempotencyKey> GetItem(Expression<Func<IdempotencyKey, bool>> predicate)
        {
            var query = _context.IdempotencyKeys.Where(predicate);
            return await query.FirstOrDefaultAsync();
        }

        public virtual async Task UpdateItem(IdempotencyKey item)
        {
            _context.IdempotencyKeys.Update(item);
            await _context.SaveChangesAsync();
        }
    }
}
