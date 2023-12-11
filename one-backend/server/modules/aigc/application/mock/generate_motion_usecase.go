package mock

import (
	"context"
	"fmt"
	"time"

	"xrspace.io/server/modules/aigc/domain"
)

const folder = "music_to_motion"

type GenerateMotionUsecase struct {
	storage domain.IStorage
}

type GenerateMotionCommand struct {
	InputUrl string `json:"input_url" binding:"required" validate:"required"`
}

func NewGenerateMotionUsecase(
	storage domain.IStorage,
) *GenerateMotionUsecase {
	return &GenerateMotionUsecase{
		storage: storage,
	}
}

func (c *GenerateMotionUsecase) Execute(ctx context.Context, cmdz any) (any, error) {
	command := cmdz.(*GenerateMotionCommand)

	// sleep 5 sec to simulate inference
	time.Sleep(5 * time.Second)

	buckets := c.storage.GetBuckets()
	defaultBucket := buckets["default"].Name

	inputPath := fmt.Sprintf("%s/%s/input/", defaultBucket, folder)
	outputPath := fmt.Sprintf("s3://%s/%s/output/", defaultBucket, folder)

	mockMap := map[string]string{
		inputPath + "Cruel Summer.wav":       outputPath + "Cruel Summer.bin",
		inputPath + "Fukumean.wav":           outputPath + "Fukumean.bin",
		inputPath + "Paint the Town Red.wav": outputPath + "Paint the Town Red.bin",
		inputPath + "Shake It Off.wav":       outputPath + "Shake It Off.bin",
	}

	objKey, found := mockMap[command.InputUrl]

	if !found {
		objKey = outputPath + "Cruel Summer.bin"
	}

	url, err := c.storage.GetUrl(objKey)

	if err != nil {
		return nil, err
	}

	return url, nil
}
