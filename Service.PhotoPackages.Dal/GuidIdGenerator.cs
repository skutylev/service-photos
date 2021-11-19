using System;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace Service.PhotoPackages.Dal
{
    public class GuidIdGenerator : ValueGenerator<string>
    {

        public override string Next(EntityEntry entry)
        {
            return Guid.NewGuid().ToString();
        }

        public override bool GeneratesTemporaryValues => false;
    }
}