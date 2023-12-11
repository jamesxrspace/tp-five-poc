package repository

type IAvatarRepository interface {
	IGetCurrentAvatarRepository
	ISaveAvatarRepository
	ISetPlayerAvatarRepository
	IListGroupAvatarsRepository
}
