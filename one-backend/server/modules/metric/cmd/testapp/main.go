package main

import (
	"net/http"
	"time"

	"github.com/gin-gonic/gin"
	"github.com/rs/zerolog/log"

	"xrspace.io/server/core/arch/domain/event"
	"xrspace.io/server/core/dependency/eventbus/inmem"
	inputEventbus "xrspace.io/server/modules/metric/port/input/eventbus"
	inputGin "xrspace.io/server/modules/metric/port/input/gin"
	outputRepo "xrspace.io/server/modules/metric/port/output/repo"
)

/*
*
this main function is for local test only
*/
func main() {
	bus := inmem.NewInMemEventBus()
	repo := outputRepo.NewPrometheusRepository()

	go func() {
		for {
			_ = bus.Publish(event.New(
				"topic_of_test",
				"test_module",
				"1.01",
				map[string]string{
					"msg": "hello",
				},
			))
			time.Sleep(2 * time.Second)
		}
	}()

	eventController := inputEventbus.NewController(bus, repo)
	err := eventController.Subscribe()
	if err != nil {
		panic(err)
	}

	ginHandler := inputGin.NewMetricController()
	g := ginHandler.Register(gin.Default())

	srv := &http.Server{
		Addr:    ":8090",
		Handler: g,
	}

	log.Fatal().Err(srv.ListenAndServe()).Msg("server error")
}
