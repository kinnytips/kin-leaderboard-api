﻿using System.Threading.Tasks;
using AutoMapper;
using kin_leaderboard_api.Entities;
using kin_leaderboard_api.Exceptions;
using kin_leaderboard_api.Repository;
using Microsoft.Extensions.Logging;

namespace kin_leaderboard_api.Services.Abstract
{
    public class AbstractService<TDto, TModel, TId> : IAppService<TModel, TId>
        where TDto : class, new() where TModel : class, new()
    {
        protected readonly IMapper Mapper;
        protected ILogger Logger;
        protected BaseRepository<TDto, TId> Repo;

        public AbstractService(ILoggerFactory loggerFactory, ApplicationContext context, IMapper mapper)
        {
            Logger = loggerFactory.CreateLogger(GetType().Name);
            Repo = new BaseRepository<TDto, TId>(context);
            Mapper = mapper;
        }

        public virtual async Task<TModel> Get(TId id)
        {
            TDto dbEntity = await Repo.GetById(id);

            if (dbEntity == null)
            {
                throw new NotFoundApiException($"{GetType().Name} id '{id}' not found");
            }

            return Mapper.Map<TDto, TModel>(dbEntity);
        }

        public virtual Task<int> Post(TModel value)
        {
            TDto entity = Mapper.Map<TModel, TDto>(value);
            return Repo.Create(entity);
        }

        public virtual async Task Put(TId id, TModel value)
        {
            TDto dbEntity = await Repo.GetById(id).ConfigureAwait(false);

            if (dbEntity == null)
            {
                throw new NotFoundApiException($"{GetType().Name} id '{id}' not found");
            }

            TDto entity = Mapper.Map(value, dbEntity);
            await Repo.SaveChanges().ConfigureAwait(false);
        }

        public virtual async Task<int> Delete(TId id)
        {
            TDto dbEntity = await Repo.GetById(id).ConfigureAwait(false);

            if (dbEntity == null)
            {
                throw new NotFoundApiException($"{GetType().Name} id '{id}' not found");
            }

            return await Repo.Delete(dbEntity).ConfigureAwait(false);
        }
    }
}