//	Copyright © 2015 Alexander Isakov. All rights reserved.
//  This file is a part of CommandShell and is licensed under MS-PL.
//  See LICENSE for details or visit http://opensource.org/licenses/MS-PL.
//	----------------------------------------------------------------------

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace CommandShell.Helpers
{
    internal static class Validator
    {
        internal static void Validate(IEnumerable<ValidationAttribute> validators, object value)
        {
            foreach (var validator in validators)
            {
                var context = new ValidationContext(value, serviceProvider: null, items: null);
                validator.Validate(value, context);
            }
        }

        internal static bool IsValid(IEnumerable<ValidationAttribute> validators, object value)
        {
            return validators.All(validator => validator.IsValid(value));
        }
    }
}
