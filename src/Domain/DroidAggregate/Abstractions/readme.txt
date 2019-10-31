Dto seems weird because it's like copying Aggregate structure.
In fact:
 - The Db can be different from Dto
 - The interface can be a subset of dto implementation: without specificities of db
In ES-CQRS we don't have dto because of event reloading directly with domain methods