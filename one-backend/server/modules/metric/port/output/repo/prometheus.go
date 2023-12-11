package repo

import (
	"fmt"
	"strings"

	"github.com/prometheus/client_golang/prometheus"
	"github.com/prometheus/client_golang/prometheus/promauto"

	"xrspace.io/server/core/arch/domain/event"
	"xrspace.io/server/modules/metric/domain/repository"
)

type PrometheusRepository struct {
	counter map[event.Topic]prometheus.Counter
}

var _ repository.IMetricRepository = (*PrometheusRepository)(nil)

func NewPrometheusRepository() *PrometheusRepository {
	return &PrometheusRepository{
		counter: make(map[event.Topic]prometheus.Counter),
	}
}

func (p *PrometheusRepository) Inc(topic event.Topic, labels map[string]string) {
	counter, ok := p.counter[topic]
	if !ok {
		help := strings.Join(strings.Split(string(topic), "_"), " ")
		counter = promauto.NewCounter(prometheus.CounterOpts{
			Name:        string(topic) + "_total",
			Help:        fmt.Sprintf("The total number of processed events: %s", help),
			ConstLabels: labels,
		})
		p.counter[topic] = counter
	}
	counter.Inc()
}
