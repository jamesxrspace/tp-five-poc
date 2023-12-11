package define

const (
	OK    = "ok"
	LOCAL = "local"
)

type InferenceResponse struct {
	OutputLocation string `json:"outputLocation"`
	Status         string `json:"status"`
	Error          string `json:"error"`
}
