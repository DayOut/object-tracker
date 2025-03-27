db.createUser(
    {
        user: "admin",
        pwd: "Test123!",
        roles: [
            {
                role: "readWrite",
                db: "UserDB"
            }
        ]
    }
);
db.createCollection("User");

db.User.insert({
    "id": "ed1625c1-e119-457d-a78f-cd898a73980e",
    "name": "admin",
    "username": "admin",
    "email": "admin",
    "phoneNumber": "admin",
    "password": "$2a$11$VFJHUdYqUzEwlx8CfLfdIuSO6WJ8.aYWqYDkxYKhAZcGx1Gmxy5YC"
});
