using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolveIt.Application.Interfaces
{
    public interface IPasswordResetTokenRepository
    {
        Task AddAsync(PasswordResetToken token, CancellationToken ct);
        Task<PasswordResetToken?> GetByHashAsync(string hash, CancellationToken ct);
        Task SaveChangesAsync(CancellationToken ct);
    }

}
