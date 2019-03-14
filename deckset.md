footer: © Thomson Reuters
slidenumbers: true

# Testing Techniques in `C#`

by Dan Jakob Ofer

---

## Who Am I?

- Software Developer @ Thomson Reuters
- Working on Content Management Systems for TR Digital since January
- Been working `here > August 2017`
- Been developing software where `age == 12 or age == 13`

---

## Why Testing?

- Known as specifications or specs
- Allows programmers to codify business requirements
- Reduces time to develop new features
- Increases design quality
- Promotes good design

---

## RSpec

- Verify Ruby software
- Testing library to verify Ruby applications using `spec`s
- Define before actions to
  - Clean and seed database **before** `spec`
  - Clean database **after** `spec`
  - And more
- `let` variables are lazily-evaluated
- Factories to facilitate creation of objects
  - Creates multiple instances
  - Reduces overhead
  - Promotes maintainability

---

### RSpec Setup

Clean the database before each test is run

```ruby
RSpec.configure do |config|
  config.before(:example) do
    DatabaseCleaner.strategy = :deletion, { except: NO_TRUNCATE_TABLES }
    DatabaseCleaner.start
  end

  config.after(:example) do
    DatabaseCleaner.clean
  end
end
```

---

### Spec Setup

```ruby
describe CreditCardStatementRecord do
  describe '#create_item!' do
    let(:statement) { FactoryGirl.create(:statement) }

    subject(:record) do
      FactoryGirl.create(:record, item_id: item_id, statement_id: statement.id)
    end

    before :each do
      FactoryGirl.create(:user, name: 'Jim Smith')
    end
```

---

### Positive Tests

```ruby
    context 'item_id is valid when only numbers are used' do
      let(:item_id) { '1234' }

      it 'does not raise error' do
        expect(subject.create_item!).not_to raise_error
      end

      it 'creates new record in database'
        expect { subject.create_item!  }
          .to change { Item.count }
          .by(1)
      end
    end
```

---

### Negative Tests

```ruby
    context 'when item_id is invalid when letters are used' do
      let(:item_id) { 'ABCD' }

      it 'throws error' do
        expect(subject.create_item!).to raise_error(DbUpdateError)
      end

      it 'does not create new record in database'
        expect do
          begin
            subject.create_item!
          rescue DbUpdateError
            # Ignore the error for the sake of the test
          end
        end.not_to change { Item.count }
      end
    end
  end
```

---

### The Power Level is Over 9000!

RSpec is great and this is why:

1. Lazily _create_ database records using `let` and `subject` variables, which are
2. Then wiped clean _after_ each test is run
3. `context` blocks are used to test against multiple values of `item_id`

---

## Testing in `C#`

So how can we write database integration tests in C#?

- Use NUnit (vs RSpec)
- Programmed in C# 7 (vs Ruby) and runs on .NET Core 2.2 (vs Matz's Ruby Interpreter)
- Use Entity Framework Core (vs ActiveRecord) against any database

---

### Terminology

Ruby / RSpec | C# / NUnit
------------ | ----------
`let`/`subject` variables | Functions
`FactoryGirl` | `Factories` class with static members
`context` block | `TestCase` attribute
`it` / `spec` block | `Test` attribute
before block to seed database | Seed method
Clean database before/after a spec | `Init`/`Cleanup` methods in `DatabaseFixture`

---

### Create an Abstract Database Fixture

- A group of test is called a fixture
- Each fixture will use the `DatabaseFixture` to facilitate
  - Wiping the database before and after each test
  - Seeding the database before each test

---

```csharp
[TestFixture]
public abstract class DatabaseFixture
{
  [OneTimeSetUp]
  public void InitAll()
  {
    Context.Environment = Environments.Test;
  }

  [SetUp]
  public void Init()
  {
    Context.Clean();

    Seed();
  }

  [OneTimeTearDown]
  public void Cleanup()
  {
    Context.Clean();
  }

  protected abstract void Seed();
}
```

---

### Concrete Fixtures

- Implement DatabaseFixture
  - Seed and clean the database before and after each test
- Define the factories (vs FactoryGirl)
- Functions are lazily-evaluated (vs `let` and `subject` variables)

---

### Factories

```csharp
public class Factories
{
  public static string NumericItemId = "1234";
  public static string AlphaItemId = "ABCD";

  public static Func<User> GetJimSmith = () => new User("Jim Smith");

  public static Func<Statement> GetStatement = () => new Statement();

  public static Func<string, Record> GetRecord = (itemId) => new Record(itemId);
}
```

---

### Seed the Database

```csharp
public class CreateItem : DatabaseFixture
{
  protected override void Seed()
  {
    using (var db = new DatabaseContext())
    {
      db.Users.Add(GetJimSmith());
      db.SaveChanges();
    }
  }
```

---

### Positive Tests

```csharp
  [Test]
  [TestCase(Factories.NumericItemId)]
  public void Test_CreateItem_ItemId_Is_Valid_Does_Not_Throw_Exception(string itemId)
  {
    Assert.DoesNotThrow<DbUpdateException>(Factories.GetRecord(itemId).CreateItem);
  }

  [Test]
  [TestCase(Factories.NumericItemId)]
  public void Test_CreateItem_ItemId_Is_Valid_CreatesItem(string itemId)
  {
    int beforeCount;
    using (var db = new Context())
    {
      beforeCount = db.Items.Count();
    }

    Factories.GetRecord(itemId).CreateItem;

    using (var db = new Context())
    {
      Assert.That(db.Items.Count() - beforeCount, 1);
      Assert.IsTrue(db.Items.Any(item => item.Id == itemId));
    }
  }
```

---

### Negative Tests

```csharp
  [Test]
  [TestCase(Factories.AlphaItemId)]
  public void Test_CreateItem_ItemId_Is_Invalid_Throws_Exception(string itemId)
  {
    Assert.Throws<DbUpdateException>(Factories.GetRecord(itemId).CreateItem);
  }

  [Test]
  [TestCase(Factories.AlphaItemId)]
  public void Test_CreateItem_ItemId_Is_Invalid_CreatesItem(string itemId)
  {
    int beforeCount;
    using (var db = new Context())
    {
      beforeCount = db.Items.Count();
    }

    Factories.GetRecord(itemId).CreateItem;

    using (var db = new Context())
    {
      Assert.That(db.Items.Count(), Is.EqualTo(beforeCount));
    }
  }
}
```

---

### Easily Add Multiple Negative Tests

```csharp
  [Test]
  [TestCase(Factories.AlphaItemId)]
  [TestCase("")]
  [TestCase(" ")]
  [TestCase(null)]
  public void Test_CreateItem_ItemId_Is_Invalid_Throws_Exception(string itemId)
  {
    Assert.Throws<DbUpdateException>(Factories.GetRecord(itemId).CreateItem);
  }
```

---

### In RSpec

```ruby
describe CreditCardStatementRecord do
  describe '#create_item!' do
    ['ABCD', '', ' ', nil].each do |item_id|
      context "when item_id is (#{item_id})" do
        subject(:record) do
          FactoryGirl.create(:record, item_id: item_id)
        end

        it 'throws exception' do
          expect(subject.create_item!).to raise_error(DbUpdateError)
        end
      end
    end
  end
end
```
