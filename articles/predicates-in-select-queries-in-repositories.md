---
title: Predicates in SELECT queries in repositories
author: Lars Rotgers
date: 2024-05-15 20:31
---

# Predicates in SELECT queries in repositories

## Tips & tricks for writing better C# code

---

A repository pattern is a common pattern in C# applications. It is used to abstract away the data access layer from the business logic layer. This allows for easier testing and swapping out the data access layer for another one. It is however quite easily to break the pattern, as we will see with the following example. 

Suppose one has a repository for a user entity: 

```cs
interface IUserRepository 
{
    User FindUserByUsername(string username);
    User FindUserByEmail(string email);

    List<User> GetUsersByAge(int age);

    // And so on...
}
```

One might argue that it is much simpler to just pass in a predicate, like so: 

```cs
interface IUserRepository
{
    User FindUser(Func predicate);
    List FindUsers(Func predicate);
}
```

With this new fancy approach, the API consumer can just match on whatever predicate they want. Easy right? Not quite, because this opens the gates for a whole host of issues. Let's take a look. 

The first issue is that when you use a `Func<User, bool>` instead of a `Expression<Func<User, bool>>` you are going to use the IEnumerable.Where instead of the IQuerable.Where, which loads in the entire table into memory. Good luck if there is a table behind it that has millions of rows. 

Alright, so we just change it into an `Expresssion<Func<User, bool>>` and we are good right? 

No. What if a user has an `IsDeleted` property on which we need to match as well? If the innocent API consumer does not know about this, they will just pass in a predicate that matches on the username, and the repository will return a deleted user. Needless to say, this is a terrible idea. 

If we allow an API consumer to pass in a predicate, suddenly the consumer of the repository needs to know all kinds of details about how a user is stored, and when a user is valid. In the case of multi-tenancy within a single database, this can result in catastrophical bugs, such as leaking customer data between customers. Now, this risk can **still** be mitigated by appending these additional checks to the end of a given `IQueryable` before passing it over to the ORM, however... 

This simplification of the repository layer transfers the complexity to the business logic layer. You end up in a situation where you can just throw the entire repository layer away and use IQueryable within the services directly. The idea of a repository layer is to be able to replace a database completely as long as you can still get the same data structure out of it. If you use higher order functions to query the database in the repository layer, you have essentially created an ORM to query your ORM. 