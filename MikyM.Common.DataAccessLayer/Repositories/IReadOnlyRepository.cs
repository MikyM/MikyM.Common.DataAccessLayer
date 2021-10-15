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

using System.Collections.Generic;
using System.Threading.Tasks;
using MikyM.Common.DataAccessLayer.Filters;
using MikyM.Common.DataAccessLayer.Specifications;
using MikyM.Common.Domain.Entities;

namespace MikyM.Common.DataAccessLayer.Repositories
{
    public interface IReadOnlyRepository<TEntity> : IBaseRepository where TEntity : AggregateRootEntity
    {
        ValueTask<TEntity> GetAsync(params object[] keyValues);

        Task<IReadOnlyList<TEntity>> GetBySpecificationsAsync(PaginationFilter filter,
            ISpecifications<TEntity> specifications = null);

        Task<IReadOnlyList<TEntity>> GetBySpecificationsAsync(ISpecifications<TEntity> specifications = null);
        Task<long> LongCountAsync(ISpecifications<TEntity> specifications = null);
    }
}