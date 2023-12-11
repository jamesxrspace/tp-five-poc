package utility

import "time"

// GetPreviousMonthLastDate returns the last date of the previous month from the given date.
// For example, if the given date is 2020-01-15, the returned date is 2019-12-31.
// if the given date is 2020-10-31, the returned date is 2020-09-30.
// the utility function prevent the bias that,
// if we use the time.AddDate(0, -1, 0), the returned date will be 2020-10-01
// not 2020-09-30, which we want.
// refence: https://learnku.com/articles/71760
func GetPreviousMonthLastDate(Date time.Time) time.Time {
	d := Date.Day()
	return Date.AddDate(0, 0, -d)
}
