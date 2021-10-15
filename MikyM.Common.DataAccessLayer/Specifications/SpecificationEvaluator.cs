﻿// This file is part of MikyM.Common.DataAccessLayer project
//
// Copyright (C) 2021 Krzysztof Kupisz - MikyM
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace MikyM.Common.DataAccessLayer.Specifications
{
    public static class SpecificationEvaluator<TEntity> where TEntity : class
    {
        public static IQueryable<TEntity> GetQuery(IQueryable<TEntity> query, ISpecifications<TEntity> specifications)
        {
            if (specifications == null) return query;

            query = specifications.FilterConditions.Aggregate(query,
                (current, filterCondition) => current.Where(filterCondition));

            query = specifications.Includes.Aggregate(query, (current, include) => current.Include(include));

            if (specifications.OrderBy != null)
                query = query.OrderBy(specifications.OrderBy);
            else if (specifications.OrderByDescending != null)
                query = query.OrderByDescending(specifications.OrderByDescending);

            if (specifications.GroupBy != null) query = query.GroupBy(specifications.GroupBy).SelectMany(x => x);

            if (specifications.Limit != 0) query = query.Take(specifications.Limit);

            return query;
        }
    }
}