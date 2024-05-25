﻿using Diary.Entities;
using Diary.Repositories.Interfaces;

namespace Diary.Repositories;

public class EntryMediaRepository : RepositoryBase<EntryMediaEntity>, IEntryMediaRepository
{
    public EntryMediaRepository() : base()
    {
    }
}
