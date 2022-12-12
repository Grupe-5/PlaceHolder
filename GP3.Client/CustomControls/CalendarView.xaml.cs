using System.Collections.ObjectModel;
using System.Windows.Input;
using GP3.Client.Models;

namespace GP3.Client.CustomControls;

public partial class CalendarView : StackLayout
{
    #region BindableProperty
    public static readonly BindableProperty SelectedDateProperty = BindableProperty.Create(
        nameof(SelectedDate),
        typeof(DateTime),
        declaringType: typeof(CalendarView),
        defaultBindingMode: BindingMode.TwoWay,
        defaultValue: DateTime.Now
        );
    #endregion

    public DateTime SelectedDate
    {
        get => (DateTime)GetValue(SelectedDateProperty);
        set => SetValue(SelectedDateProperty, value);
    }

    public ObservableCollection<CalendarModel> Dates { get; set; } = new ObservableCollection<CalendarModel>();
    public CalendarView()
	{
		InitializeComponent();
        BindDates(DateTime.Today);
    }
    private void BindDates(DateTime selectedDate)
    {
        for (int i = 0; i < 8; i++)
        {
            Dates.Add(new CalendarModel
            {
                Date = new DateTime(selectedDate.Year, selectedDate.Month, selectedDate.Day)
            });
            selectedDate = selectedDate.AddDays(-1);

        }
    }

    #region Commands
    public ICommand CurrentDateCommand => new Command<CalendarModel>((currentDate) =>
    {
        SelectedDate = currentDate.Date;
        Dates.ToList().ForEach(f => f.IsCurrentDate = false);
        currentDate.IsCurrentDate= true;
    });
    #endregion

}