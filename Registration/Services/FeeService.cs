using Microsoft.EntityFrameworkCore;
using Registration.Data;
using Registration.DTOs.Fees;

namespace Registration.Services
{
    public class FeeService : IFeeService
    {
        private readonly ApplicationDbContext _context;

        public FeeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<FeeResponse> CreateAsync(
            CreateFeeRequest request)
        {
            var studentExists = await _context.Students
                .AnyAsync(s => s.Id == request.StudentId);

            if (!studentExists)
                throw new KeyNotFoundException("Student not found.");

            var fee = new Models.Fee
            {
                StudentId = request.StudentId,
                Amount = request.Amount,
                PaymentDate = request.PaymentDate,
                Status = request.Status
            };

            _context.Fees.Add(fee);
            await _context.SaveChangesAsync();

            return new FeeResponse
            {
                Id = fee.Id,
                StudentId = fee.StudentId,
                Amount = fee.Amount,
                PaymentDate = fee.PaymentDate,
                Status = fee.Status
            };
        }

        public async Task<List<FeeResponse>> GetByStudentIdAsync(
            int studentId)
        {
            return await _context.Fees
                .AsNoTracking()
                .Where(f => f.StudentId == studentId)
                .OrderByDescending(f => f.PaymentDate)
                .Select(f => new FeeResponse
                {
                    Id = f.Id,
                    StudentId = f.StudentId,
                    Amount = f.Amount,
                    PaymentDate = f.PaymentDate,
                    Status = f.Status
                })
                .ToListAsync();
        }
    }
}