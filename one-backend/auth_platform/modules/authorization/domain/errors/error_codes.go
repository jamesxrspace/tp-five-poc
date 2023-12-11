package auth_errors

var (
	InvalidArguments          = 10001
	InvalidUsernameOrPassword = 10002
	InvalidToken              = 10003
	InvalidRefreshToken       = 10004
	UnableGetJwks             = 10005
	SignTokenFailed           = 10006
	GenRandFailed             = 10007
	JsonMarshalFailed         = 10008
	ParseTokenFailed          = 10009
	TokenExpired              = 10010
	ReadPrivateKeyFailed      = 10011
	ParsePrivateKeyFailed     = 10012
	ReadPublicKeyFailed       = 10013
	ParsePublicKeyFailed      = 10014
	InvalidSignMethod         = 10015
	GetAccountInfoFailed      = 10016
	UserIdNotMatch            = 10017
	FindProfileFailed         = 10018
	InvalidEmailOrPassword    = 10019
	GenTokenFailed            = 10020
	GenRefreshTokenFailed     = 10021
	DecodeClaimsFailed        = 10022
	UpdateProfileFailed       = 10023
	EmailDuplicated           = 10024
	CreateUserFailed          = 10025
	SaveUserFailed            = 10026
)
