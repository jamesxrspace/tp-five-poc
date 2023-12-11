package main

import (
	"context"
	"net/http"
	"os"

	"github.com/gin-gonic/gin"
	"github.com/rs/zerolog"
	"github.com/rs/zerolog/pkgerrors"

	"xrspace.io/server/core/arch/example/application"
	mongo2 "xrspace.io/server/core/arch/example/port/repository/mongo"
	gin2 "xrspace.io/server/core/arch/port/gin"
	"xrspace.io/server/core/dependency/database/docdb"
	"xrspace.io/server/core/dependency/log"
)

func main() {

	db, server, err := docdb.NewMemgoDocDB(context.Background())
	if err != nil {
		panic(err)
	}

	defer server.Stop()

	// using generic repository
	repo := mongo2.NewRoomRepository(db)
	facade := application.NewAFacade(repo)

	g := gin.Default()
	g.GET("/join", gin2.HandlerFunc[application.JoinCommand](facade))
	g.GET("/leave", gin2.HandlerFunc[application.LeaveCommand](facade))
	g.GET("/error", gin2.HandlerFunc[application.ErrorCommand](facade))

	s := &http.Server{
		Addr:    ":8080",
		Handler: g,
	}

	/**
	zerolog provides a default stack trace marshaling function.
	but the file path is not included in the stack trace.
	So we need to provide our own stack trace marshaling function.
	You could set the Env=local to enable the file path in the stack trace.
	And `curl localhost:8080/error` to see the stack trace.
	*/
	if os.Getenv("Env") == "local" {
		zerolog.ErrorStackMarshaler = log.DebugMarshalStack
	} else {
		zerolog.ErrorStackMarshaler = pkgerrors.MarshalStack
	}

	err = s.ListenAndServe()
	if err != nil {
		panic(err)
	}

}
