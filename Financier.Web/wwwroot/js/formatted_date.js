class FormattedDate extends Date {
  constructor(year, month, day) {
    super(year, month, day);
  }

  get toFormatted() {
    var day = this.getDate().toString();
    if (this.getDate() < 10) {
      day = `0${this.getDate()}`;
    }

    var months = this.months;
    var month = months[this.getMonth()];

    return `${day} ${month} ${this.getFullYear()}`;
  }

  get months() {
    return [
      'January',
      'February',
      'March',
      'April',
      'May',
      'June',
      'July',
      'August',
      'September',
      'October',
      'November',
      'December'
    ];
  }
}

export { FormattedDate };
