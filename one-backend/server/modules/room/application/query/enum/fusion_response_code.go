package enum

// FusionResponseCode Documentation: https://doc.photonengine.com/fusion/v2/manual/connection-and-matchmaking/custom-authentication
// Session "Returning Data To Client"
type FusionResponseCode int

const (
	FusionResponseCodeIncomplete       FusionResponseCode = 0
	FusionResponseCodeSuccess          FusionResponseCode = 1
	FusionResponseCodeFailure          FusionResponseCode = 2
	FusionResponseCodeInvalidParameter FusionResponseCode = 3
)
