class Element {
  constructor(selector) {
    this.selector = selector;
  }

  getContainer() {
    return $(this.selector);
  }

  clear() {
    this.getContainer().empty();
  }

  appendNewChild(element) {
    this.getContainer().append(element);
  }

  getAbsoluteSelectorOfExistingChild(relativeSelector) {
    return `${this.selector} ${relativeSelector}`;
  }

  // as(klass) {
  //   switch (klass) {
  //     case Input:
  //       return new Input(this.selector);
  //     case Element:
  //       return new Element(this.selector);
  //     case Button:
  //       return new Button(this.selector);
  //     case DatePicker:
  //       return new DatePicker(this.selector):
  //     case Payments:
  //       return new Payments(this.selector);
  //     case Payment:
  //       return new Payment(this.selector);
  //     default:
  //       return this;
  //   }
  // }
}

class ActionableElement extends Element {
  constructor(selector) {
    super(selector);
    // TODO: rename to something else that fits the applied design pattern
    this.subscriptions = [];
  }

  subscribe(func) {
    this.subscriptions.push(func);
  }

  execute() {
    this.subscriptions.forEach(item => item());
  }
}

class Input extends Element {
  constructor(selector) {
    super(selector);
  }

  get value() {
    return this.getContainer().val();
  }

  set value(val) {
    return this.getContainer().val(val);
  }
}

class Form extends Element {
  // TODO: inject the results component
  constructor(selector) {
    super(selector);

    this.compareButton = new Button(".compare input");
    this.clearButton = new Button(".clear input");
  }

  getInput(name) {
    var selector = this.getAbsoluteSelectorOfExistingChild(`.${name} input`);

    return new Input(selector);
  }

  setSubmitButton(action) {
    this.compareButton.subscribe(action);
  }

  setClearButton(action) {
    this.clearButton.subscribe(action);
  }

  render() {
    this.compareButton.render();
    this.clearButton.render();
    this.purchasedAt.render();
  }

  clear() {
    this.compareButton.clear();
    this.clearButton.clear();
    this.purchasedAt.clear();
  }
}

class Button extends ActionableElement {
  constructor(selector) {
    super(selector);
  }

  render() {
    this.getContainer().click(() => this.execute());
  }
}

class DatePicker extends Element {
  constructor(selector, value) {
    super(selector);

    this.value = value;
  }

  render() {
    // FIXME: use the getContainer function
    this.getContainer().datepicker();
    this.getContainer().val(`${this.value.getMonth()}/${this.value.getDay()}/${this.value.getFullYear()}`);
  }

  val() {
    return new Date(this.year, this.month - 1, this.day);
  }

  toString() {
    return this.getContainer().val()
      .split("/")
      .join("-");
  }

  toArray() {
    return this.toString()
      .split("/")
      .map(v => +v);
  }

  get day() {
    return this.toArray()[1];
  }

  get month() {
    return this.toArray()[0];
  }

  get year() {
    return this.toArray()[2];
  }

  clear() {
    this.getContainer().val("");
  }
}

class Payments extends Element {
  constructor(selector, data) {
    super(selector);

    var data = _.map(data, item => {
      var at = item["at"]
        .split("-")
        .map(v => +v);

      return {
        "at": new Date(at[0], at[1] - 1, at[2]),
        "amount": item["amount"]
      };
    });

    this.data = _.orderBy(data, item => item["at"], "asc");
  }

  render() {
    var container = this.getContainer();

    this.data.forEach(item => {
      var id = Math.floor(Math.random() * 1000000).toString();
      var element = `
            <div id="${id}" class="payment" />
          `;
      container.append(element);

      new Payment(`.payment#${id}`, item).render();
    });
  }
}

class Payment extends Element {
  constructor(selector, data) {
    super(selector);

    this.data = data;
  }

  render() {
    var container = this.getContainer();
    var identifer = this.data["at"];
    var paymentElement = `
          <div class="payment">
            <h1>${this.data["at"]}</h1>

            <div class="at">
              ${this.data["at"]}
            </div>

            <div class="amount">
              ${this.data["amount"]}
            </div>
          </div>
        `;

    container.append(paymentElement);
    //
    // this
    //   .createPaymentElement(`.items#${identifer}`, item.items)
    //   .render();
  }
}

export { Element, ActionableElement, Input, Form, Button, DatePicker };
