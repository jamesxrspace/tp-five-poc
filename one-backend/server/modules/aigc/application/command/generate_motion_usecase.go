package command

import (
	"context"
	"encoding/json"
	"fmt"
	"time"

	"github.com/go-playground/validator/v10"
	"xrspace.io/server/core/arch/core_error"
	"xrspace.io/server/modules/aigc/domain"
	"xrspace.io/server/modules/aigc/domain/define"
	"xrspace.io/server/modules/aigc/domain/error_code"
)

const (
	timeout        = 60 * time.Second
	completedState = "Completed"
	failedState    = "Failed"
)

type GenerateMotionUsecase struct {
	queue      domain.IQueue
	storage    domain.IStorage
	inferencer domain.IInferencer
}

type GenerateMotionCommand struct {
	InputUrl string `json:"input_url" binding:"required" validate:"required"`
}

func NewGenerateMotionUsecase(
	queue domain.IQueue,
	storage domain.IStorage,
	inferencer domain.IInferencer,
) *GenerateMotionUsecase {
	return &GenerateMotionUsecase{
		queue:      queue,
		storage:    storage,
		inferencer: inferencer,
	}
}

func (c *GenerateMotionUsecase) Execute(ctx context.Context, cmdz any) (any, error) {
	command := cmdz.(*GenerateMotionCommand)
	requestID, err := c.inferencer.Inference(ctx, command.InputUrl)

	if err != nil {
		err = fmt.Errorf("[Motion] Inference InputUrl: %q: %w", command.InputUrl, err)
		return nil, core_error.NewCoreError(
			error_code.InferenceError,
			err,
		)
	}

	msg, err := c.queue.BlockLeftPop(ctx, requestID, timeout)

	if err != nil {
		err = fmt.Errorf("[Motion] BLPop RequestID: %q: %w", requestID, err)
		return nil, core_error.NewCoreError(
			error_code.QueuePopError,
			err,
		)
	}

	response, err := parseMsg(msg)

	if err != nil {
		return nil, core_error.NewCoreError(
			error_code.ParseMsgError,
			err,
		)
	}

	if response.Status == failedState {
		return nil, core_error.NewCoreError(
			error_code.InferenceError,
			core_error.NewInternalError(response.Error),
		)
	}

	url, err := c.storage.GetUrl(response.OutputLocation)

	if err != nil {
		err = fmt.Errorf("[Motion] Get url OutputLocation: %q: %w", response.OutputLocation, err)
		return nil, core_error.NewCoreError(
			error_code.GetUrlError,
			err,
		)
	}

	return url, nil
}

func parseMsg(msg string) (*define.InferenceResponse, error) {
	var result define.InferenceResponse
	err := json.Unmarshal([]byte(msg), &result)
	if err != nil {
		return nil, core_error.NewInternalError(err)
	}

	validate := validator.New()
	if err := validate.Struct(result); err != nil {
		return nil, core_error.NewInternalError(err)
	}

	return &result, nil
}
