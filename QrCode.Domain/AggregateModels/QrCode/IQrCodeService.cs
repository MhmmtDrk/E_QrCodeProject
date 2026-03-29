using QrCode.Domain.AggregateModels.TagModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QrCode.Domain.AggregateModels.QrCode
{
    public interface IQrCodeService
    {
        Task<QrCode> CreateAsync(Guid userId, string no, string verificationCode, string description, bool isVerification, bool isActive, CancellationToken cancellationToken = default);
    }
}
