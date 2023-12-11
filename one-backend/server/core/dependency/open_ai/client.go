package open_ai

import (
	"context"

	"github.com/rs/zerolog"
	openai "github.com/sashabaranov/go-openai"
	"xrspace.io/server/core/arch/core_error"
	"xrspace.io/server/core/dependency/settings"
)

type OpenAIClient struct {
	Client *openai.Client
}

func NewOpenAIClient(config *settings.OpenAiConfig) *OpenAIClient {
	c := openai.DefaultConfig(config.ApiKey)
	c.OrgID = config.OrganizationID
	client := openai.NewClientWithConfig(c)

	return &OpenAIClient{
		Client: client,
	}
}

func (o *OpenAIClient) ListModels(ctx context.Context) (models openai.ModelsList, err error) {
	models, err = o.Client.ListModels(ctx)
	if err != nil {
		return models, core_error.StackError(err)
	}
	return models, nil
}

func (o *OpenAIClient) IsModelExisted(ctx context.Context, modelID string) bool {
	_, err := o.Client.GetModel(ctx, modelID)
	if err != nil {
		log := zerolog.Ctx(ctx)
		log.Info().Err(err).Msgf("Model %s not found", modelID)
		return false
	}
	return true
}

func (o *OpenAIClient) StreamText(ctx context.Context, request openai.ChatCompletionRequest, text string) (<-chan string, error) {
	log := zerolog.Ctx(ctx)
	responseTextChan := make(chan string)
	stream, streamErr := o.Client.CreateChatCompletionStream(ctx, request)
	if streamErr != nil {
		defer close(responseTextChan)
		log.Info().Err(core_error.StackError(streamErr)).Msg("[OpenAI Client] CreateChatCompletionStream error")
		return responseTextChan, streamErr
	}

	go func() {
		defer close(responseTextChan)
		defer stream.Close()
		for {
			var response openai.ChatCompletionStreamResponse
			response, streamErr = stream.Recv()
			if streamErr != nil {
				log.Info().Err(core_error.StackError(streamErr)).Msg("[OpenAI Client] ChatCompletionStreamResponse error")
				break
			}

			if len(response.Choices) > 0 {
				responseTextChan <- response.Choices[0].Delta.Content
			}
		}
	}()

	return responseTextChan, nil
}
