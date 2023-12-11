package entity

type XrID string
type User struct {
	XrID XrID `json:"xrid" bson:"xrid"`
}

func NewUser(xrid string) *User {
	return &User{
		XrID: XrID(xrid),
	}
}
