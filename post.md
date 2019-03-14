# Testing in C#

## Who Am I?

- By Dan Jakob Ofer
- Software Developer @ Thomson Reuters
- Been working here > August 2017
- Been developing software in > age = 13 (or 12, I do not remember)

## Why Testing?

- Testing is important
- Allows programmers to codify business requirements
- Reduce the time to develop new features
- Increase the design quality of the software
- Encourages good design in software development
- You should do it!

## RSpec

- Testing library for writing concise `spec`s to verify source code functionality
- Define before actions to 
  - set up database before `spec`
  - post-actions after `spec`
- `let` variable that are lazily-evaluated
- Factories to facilitate creation of objects
  - create multiple instances
  - reduce overhead
  - increase maintainability

### Setup Tests

- Clean database before and each test is run

```ruby
RSpec.configure do |config|
  config.before(:suite) do
    DatabaseCleaner.clean_with(:truncation, except: NO_TRUNCATE_TABLES)
  end
end

config.before(:example) do
  DatabaseCleaner.strategy = :transaction
end

config.before(:example, truncate_db: true) do
  DatabaseCleaner.strategy = :deletion, { except: NO_TRUNCATE_TABLES }
end

config.before(:example, type: :feature) do
  DatabaseCleaner.strategy = :deletion, { except: NO_TRUNCATE_TABLES }
end

config.after(:example) do
  DatabaseCleaner.clean
end
```

### Tests

- Validate functionality of Ruby application using RSpec `spec`s

```ruby
describe CreditCardStatementRecord do
  describe '#create_item' do
    let(:statement) { FactoryGirl.create(:statement) }

    subject(:record) do
      FactoryGirl.create(:record, item_id: item_id)
    end

    before :each do
      FactoryGirl.create(:user, name: 'Jim Smith')
    end

    context 'item_id is valid when only numbers are used' do
      let(:item_id) { '1234' }

      it 'does not error' do
        expect(get_record.create_item!).not_to raise_error
      end

      it 'creates new record in database'
        expect { get_record.create_item!  }
          .to change { Item.count }
          .by(1)
      end
    end

    context 'when item_id is invalid when letters are used' do
      let(:item_id) { 'ABCD' }

      it 'throws exception' do
        expect(get_record.create_item!).to raise_error(DbUpdateError)
      end

      it 'does not create new record in database'
        expect do
          begin
            get_record.create_item!
          rescue DbUpdateError
            # Ignore the error for the sake of the test
          end
        end.not_to change { Item.count }
      end
    end
  end
```

So there are several characteristics of this RSpec code that I like. That I find particularly appealling:

1. Creating lazily create database records for each `it` test
2. The database is wiped clean before and after each `it` test
3. Can test multiple conditions of item_id

## C#

So how can we write database integration tests in C#?

Well in C# I use C# 7.0, .NET Core 2.2, and Entity Framework Core, and I use NUnit 3 as a testing library

### Create a database fixtures

- Tests are organised into fixtures
- I create a DatabaseFixture that
  - contains lazily-evaluated database models
  - wipes the database before and after each test
  - Seeds the database before each test

```csharp
[TestFixture]
public abstract class DatabaseAbstractFixture
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

    InitDb();
  }

  [OneTimeTearDown]
  public void Cleanup()
  {
    Context.Clean();
  }

  protected abstract void InitDb();
}
```

### Individual Fixtures

- define the factories (i.e., the `let` and `subject` variables)

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

- Intialises the database before each test

```csharp
public class CreateItem : DatabaseAbstractFixture
{
  protected override void InitDb()
  {
    using (var db = new DatabaseContext())
    {
      db.Users.Add(GetJimSmith());
      db.SaveChanges();
    }
  }
}
```

- And then we can create the tests

```
[Test]
[TestCase(Factories.NumericItemId)]
public void Test_CreateItem_ItemId_Is_Valid_Does_Not_Throw_Error(string itemId)
{
  Assert.DoesNotThrow<DbUpdateError>(Factories.GetRecord(itemId).CreateItem);
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

  [Test]
  [TestCase(Factories.AlphaItemId)]
  public void Test_CreateItem_ItemId_Is_Invalid_Throws_Error(string itemId)
  {
    Assert.Throws<DbUpdateError>(Factories.GetRecord(itemId).CreateItem);
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
