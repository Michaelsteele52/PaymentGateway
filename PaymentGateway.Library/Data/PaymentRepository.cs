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
    public class PaymentRepository : IDbRespository<PaymentDetails>
    {
        private readonly PaymentsContext _context;

        public PaymentRepository(PaymentsContext context)
        {
            _context = context;
        }

        public virtual async Task AddNewItem(PaymentDetails item)
        {
            await _context.PaymentDetails.AddAsync(item);
            await _context.SaveChangesAsync();
        }

        public virtual async Task<PaymentDetails> GetItem(Expression<Func<PaymentDetails, bool>> predicate)
        {
            var query = _context.PaymentDetails.Include(item => item.PaymentMethod).Where(predicate);
            return await query.FirstOrDefaultAsync();
        }

        public virtual async Task UpdateItem(PaymentDetails item)
        {
            _context.PaymentDetails.Update(item);
            await _context.SaveChangesAsync();
        }
    }
}
