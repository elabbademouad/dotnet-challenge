using Cds.DroidManagement.Domain.DroidAggregate.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace Cds.DroidManagement.Api.Extensions
{
    internal static class BusinessExceptionExtension
    {
        private static Dictionary<Type, StatusCodeResult> ExceptionManagement =>
            new Dictionary<Type, StatusCodeResult>
            {
                { typeof(DroidNotFoundException), new NotFoundResult() },
                { typeof(DroidConflictNameException), new ConflictResult() },
                { typeof (DroidTooManyArmsException), new ConflictResult() }
            };

        internal static StatusCodeResult GetStatusCode(this BusinessException ex) => ExceptionManagement[ex.GetType()];
    }
}
