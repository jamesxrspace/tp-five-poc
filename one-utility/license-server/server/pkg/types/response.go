package types

// the response after lambda invocation.
type Response struct {
	ID string `json:"id"`

	Email    string `json:"email"`
	Password string `json:"password"`
	Serial   string `json:"serial"`
}

func FakeResponse() *Response {
	return &Response{
		ID:       "1234567890",
		Email:    "user@example.com",
		Password: "password",
		Serial:   "1234567890",
	}
}
