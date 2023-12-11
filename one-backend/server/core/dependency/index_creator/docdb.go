package index_creator

import (
	"context"

	"github.com/rs/zerolog/log"

	"xrspace.io/server/core/dependency/database/docdb"
	accountMongo "xrspace.io/server/modules/account/port/output/repository/mongo"
	uploadMongo "xrspace.io/server/modules/asset/port/output/repository/mongo"
	avatarMongo "xrspace.io/server/modules/avatar/port/output/repository/mongo"
	feedMongo "xrspace.io/server/modules/feed/port/output/repository/mongo"
	reactionMongo "xrspace.io/server/modules/reaction/port/output/repository/mongo"
	reelMongo "xrspace.io/server/modules/reel/port/output/repository/mongo"
)

type IndexCreator struct {
	db *docdb.DocDB
}

func NewIndexCreator(db *docdb.DocDB) *IndexCreator {
	return &IndexCreator{
		db: db,
	}
}

func (c *IndexCreator) CreateIndexes(ctx context.Context) error {
	accountRepository := accountMongo.NewQueryRepository(c.db)

	if err := accountRepository.InitIndex(ctx); err != nil {
		log.Error().Err(err).Msg("create index error")
		return err
	}

	var avatarRepository = avatarMongo.NewAvatarRepository(c.db, nil)
	if err := avatarRepository.InitIndex(ctx); err != nil {
		log.Error().Err(err).Msg("create avatar repository")
		return err
	}

	var reelRepository = reelMongo.NewQueryRepository(c.db)
	if err := reelRepository.InitIndex(ctx); err != nil {
		log.Error().Err(err).Msg("create reel repository")
		return err
	}

	var feedRepository = feedMongo.NewQueryRepository(c.db)
	if err := feedRepository.InitIndex(ctx); err != nil {
		log.Error().Err(err).Msg("create feed repository")
		return err
	}

	var uploadRepository = uploadMongo.NewUploadRepository(c.db)
	if err := uploadRepository.InitIndex(ctx); err != nil {
		log.Error().Err(err).Msg("create upload repository")
		return err
	}

	var reactionRepository = reactionMongo.NewQueryRepository(c.db)
	if err := reactionRepository.InitIndex(ctx); err != nil {
		log.Error().Err(err).Msg("create reaction repository")
		return err
	}

	log.Info().Msg("create index success")
	return nil
}
