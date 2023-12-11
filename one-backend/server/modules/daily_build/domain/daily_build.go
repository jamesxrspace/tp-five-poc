package domain

import (
	"time"
)

type DailyBuild struct {
	Key       string    `json:"key"`
	Date      time.Time `json:"date"`
	BuildType BuildType `json:"build_type"`
	Url       string    `json:"url"`
	Filepath  string    `json:"file_path"`
}

// implement sort.Interface
type DailyBuilds []*DailyBuild

func (d DailyBuilds) Len() int {
	return len(d)
}

func (d DailyBuilds) Less(i, j int) bool {
	return d[i].Date.After(d[j].Date)
}

func (d DailyBuilds) Swap(i, j int) {
	d[i], d[j] = d[j], d[i]
}
