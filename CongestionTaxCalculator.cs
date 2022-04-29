using System;

namespace congestion.calculator
{
    public class CongestionTaxCalculator
    {

        public int GetTax(IVehicle vehicle, DateTime[] dates)
        {
            DateTime intervalStart = dates[0];
            var totalFee = 0;
            foreach (DateTime date in dates)
            {
                var nextFee = GetTollFee(date, vehicle);
                var tempFee = GetTollFee(intervalStart, vehicle);

                long diffInMillies = date.Millisecond - intervalStart.Millisecond;
                var minutes = diffInMillies / 1000 / 60;

                if (minutes <= 60)
                {
                    if (totalFee > 0) totalFee -= tempFee;
                    if (nextFee >= tempFee) tempFee = nextFee;
                    totalFee += tempFee;
                }
                else
                {
                    totalFee += nextFee;
                }
            }
            if (totalFee > 60) totalFee = 60;
            return totalFee;
        }

        private bool IsTollFreeVehicle(IVehicle vehicle)
        {
            if (vehicle == null) return false;
            var vehicleType = vehicle.GetVehicleType();
            return vehicleType.Equals(TollFreeVehicles.Motorcycle.ToString()) ||
                   vehicleType.Equals(TollFreeVehicles.Tractor.ToString()) ||
                   vehicleType.Equals(TollFreeVehicles.Emergency.ToString()) ||
                   vehicleType.Equals(TollFreeVehicles.Diplomat.ToString()) ||
                   vehicleType.Equals(TollFreeVehicles.Foreign.ToString()) ||
                   vehicleType.Equals(TollFreeVehicles.Military.ToString());
        }

        public int GetTollFee(DateTime date, IVehicle vehicle)
        {
            if (IsTollFreeDate(date) || IsTollFreeVehicle(vehicle)) return 0;

            int hour = date.Hour;
            int minute = date.Minute;

            if (hour == 6 && minute is >= 0 and <= 29) return 8;
            if (hour == 6 && minute is >= 30 and <= 59) return 13;
            if (hour == 7 && minute is >= 0 and <= 59) return 18;
            if (hour == 8 && minute is >= 0 and <= 29) return 13;
            if (hour is >= 8 and <= 14 && minute is >= 30 and <= 59) return 8;
            if (hour == 15 && minute is >= 0 and <= 29) return 13;
            if (hour == 15 && minute >= 0 || hour == 16 && minute <= 59) return 18;
            if (hour == 17 && minute is >= 0 and <= 59) return 13;
            if (hour == 18 && minute is >= 0 and <= 29) return 8;
            return 0;
        }

        private Boolean IsTollFreeDate(DateTime date)
        {
            var year = date.Year;
            var month = date.Month;
            var day = date.Day;

            if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday) return true;

            if (year == 2013 && (month == 1 && day == 1 ||
                                 month == 3 && (day == 28 || day == 29) ||
                                 month == 4 && (day == 1 || day == 30) ||
                                 month == 5 && (day == 1 || day == 8 || day == 9) ||
                                 month == 6 && (day == 5 || day == 6 || day == 21) ||
                                 month == 7 ||
                                 month == 11 && day == 1 ||
                                 month == 12 && (day == 24 || day == 25 || day == 26 || day == 31)))
            {
                return true;
            }
            return false;
        }

        private enum TollFreeVehicles
        {
            Motorcycle = 0,
            Tractor = 1,
            Emergency = 2,
            Diplomat = 3,
            Foreign = 4,
            Military = 5
        }
    }
}