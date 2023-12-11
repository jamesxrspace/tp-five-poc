package utility

import (
	"testing"
	"time"
)

func TestGetPreviousMonthLastDate(t *testing.T) {
	testCases := []struct {
		date     time.Time
		expected time.Time
	}{
		{time.Date(2020, 1, 1, 0, 0, 0, 0, time.UTC), time.Date(2019, 12, 31, 0, 0, 0, 0, time.UTC)},
		{time.Date(2020, 1, 31, 0, 0, 0, 0, time.UTC), time.Date(2019, 12, 31, 0, 0, 0, 0, time.UTC)},
		{time.Date(2020, 2, 29, 0, 0, 0, 0, time.UTC), time.Date(2020, 1, 31, 0, 0, 0, 0, time.UTC)},
		{time.Date(2020, 2, 28, 0, 0, 0, 0, time.UTC), time.Date(2020, 1, 31, 0, 0, 0, 0, time.UTC)},
		{time.Date(2020, 3, 31, 0, 0, 0, 0, time.UTC), time.Date(2020, 2, 29, 0, 0, 0, 0, time.UTC)},
		{time.Date(2020, 4, 30, 0, 0, 0, 0, time.UTC), time.Date(2020, 3, 31, 0, 0, 0, 0, time.UTC)},
		{time.Date(2020, 5, 31, 0, 0, 0, 0, time.UTC), time.Date(2020, 4, 30, 0, 0, 0, 0, time.UTC)},
		{time.Date(2020, 6, 30, 0, 0, 0, 0, time.UTC), time.Date(2020, 5, 31, 0, 0, 0, 0, time.UTC)},
		{time.Date(2020, 7, 31, 0, 0, 0, 0, time.UTC), time.Date(2020, 6, 30, 0, 0, 0, 0, time.UTC)},
		{time.Date(2020, 8, 31, 0, 0, 0, 0, time.UTC), time.Date(2020, 7, 31, 0, 0, 0, 0, time.UTC)},
		{time.Date(2020, 9, 30, 0, 0, 0, 0, time.UTC), time.Date(2020, 8, 31, 0, 0, 0, 0, time.UTC)},
		{time.Date(2020, 10, 31, 0, 0, 0, 0, time.UTC), time.Date(2020, 9, 30, 0, 0, 0, 0, time.UTC)},
		{time.Date(2020, 11, 30, 0, 0, 0, 0, time.UTC), time.Date(2020, 10, 31, 0, 0, 0, 0, time.UTC)},
		{time.Date(2020, 12, 31, 0, 0, 0, 0, time.UTC), time.Date(2020, 11, 30, 0, 0, 0, 0, time.UTC)},
	}

	for _, testCase := range testCases {
		actual := GetPreviousMonthLastDate(testCase.date)
		if actual != testCase.expected {
			t.Errorf("For date %v, expected %v but got %v", testCase.date, testCase.expected, actual)
		}
	}
}
