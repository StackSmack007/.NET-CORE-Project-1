# .NET-CORE-Project-1
My first web application
About the author:

Nikolay Hristov

Email: niki.hristov007@gmail.com

Skype: niksana3

About the project and GDPR:
This web app is online shop. It has educational purpose and it is not meant for commercial deals.
All of the prices, quantities and categories are randomly chosen.
If purchase is generated it will lead to generating an email for the buyer but no financial-or any kind of commercial obligations will occur.
Due to the educational purpose of this app GDPR is not implemented at the current state.

Main functionality of Junjuria Store:
1. User Roles and permissions:
1.1.Guest: (non logged in user)

Guest can:
+ Visit and explore all items (both as listings and details).
+ Search for products by name (case insensitive 2 letters match).
+ See all rankings of products, most purchased, most commented, by category, by manufacturer...
+ See who rated particular product.
+ View manufacturer's data, categories by products and overall app info on index-page
+ Add products to his basket but if he wishes to "Finalize order" he will be asked to log in first.
+ Manipulate product quantity from nav bar.
However if item's stock quantity is not enough item quantity will not be allowed to be increased.
+ View AboutUs (this page) info.
+ Write recomendation to the web app by choosing name topic and content.
1.2. User: (logged in user)

User Credentials:
UserName: [User1] [User2] [User3] [User4] [User5]

Password: [123456a]

User have all the privileges of guest.
User can:
+ "Finalize order" and change product quanitities in the order additionally.
When choosing this option products quantities are updated and listed.
if some quantity is less available than demanded it will be displayed with it's maximum stock quantity instead.
When submit order is chosen it will proceed only if quantities are available,
otherwise Finalize-Order page will refresh with available quantities only.
+ Rate Products. User can rate product after which he can change his rating but not remain neutral any more.
+ Write comments to products. User can not write comment if the last written comment is his own.
+ Give attitude to comments of products (Like or Dislike). User can give only 1 attitude point per comment if he likes comment he can dislike it but he can no longer stay neutral
User can like his own comment
+ Can view his orders and monitor orders status. Also user is notified by email upon order-status change. + Can receive email upon submitting an order. + Can view his own products rankings: Commented, Favourite and Rated products. + Can view his warranties of products he ordered /only if status of order is finalised/. + Can address service staff by chatting with them.
When user writes to service all service staff can see or respond.
+ Write recommendation to the web app by choosing name or using his own by default.
1.3. Assistant: (logged in assistant)

Assistant Credentials:
UserName: [Assistance1] [Assistance2]

Password: [123456a]

Assistant have all the privileges of User.
Assistant can:
+ Can view all orders of all users and change their status
If order is with status cancelled or finalised it can not be manipulated further.
+ Can provide assistance by responding to all users who go to Contact Us section.
+ Can view who of the users are seeking help and who of the other staff members is on.
+ Can receive mass messages from Admin.
1.4. Admin: (logged in admin)

Admin Credentials:
UserName: [Admin1] [Admin2]

Password: [123456a]

Admin have all the privileges of Assistant.
Admin can:
+ Add new category.
+ Edit categories: /Changing Title, Description or Parent category./
+ Remove category if no products or child categories are in it./otherwise he must relocate content of category before deletion/
+ Add new Manufacturer.
+ Edit Manufacturer: /Changing information about manufacturer/
+ Remove Manufacturer only if manufacturer has no products. Removal is reversible!
+ Add new Product.
+ Edit Product: This includes removing of comments, editing of comments and resetting of product's user Rating.
+ Change stock quantity of product.
+ Remove Product - reversible
+ Write to users who seek chat-aid.
+ Send mass messages to all users in chat.
+ Send mass messages to Assistants and Admins in chat via -Administrator panel.
+ Hide inapropriate recomendations from users in recomendations Tab.
+ Send mass messages to all users in chat.
+ Send mass messages to Assistants and Admins in chat via -Administrator panel.
2. About implementation key aspects:
2.1. Basket management

Basket is implemented by using User session upon submission of order.
Locking of Products DB occures and checking if quantities in basket are available, if false refreshing of order details for user to choose his further actions.
If they are available order is created.
2.2. Nav button Categories (sidebar)

Categories can be nested. Every product can be present in one category.
The number of each category represents the ammount of products not deleted in the category and all of it's children categories.
The menu is loaded from DB and stored in Memory Cache. It loads recursively the nested categories
by generating html-code in RazorView. However if Admin modifies category somehow cash-memory is deleted and thus reseted giving UI update upon refreshing.
2.3. Nav button Manufacturers (sidebar)

Similar to Categories but no nesting is involved, also stored in Memory Cache.
The number represents the non-deleted products of particular Manufacturer.

2.4. Managing product pictures and reviews

Reviews are expected to be provided as youtube video url's upon registering a product. The url is stored in DB and launched as embedded video
Pictures are expected to be provided as foreign url links. Upon creation the app makes a copy of the image in its 'own cloudinery API store account and saves its url in database.

2.5. Emailing users

Emailing is implemented by SendInBlue SMTP account
2.6. Favourizing product

Favourizing product stores product in DB-mapping table when button is clicked it uses JS ajax call.
2.7. Contact Us - chat

It uses SignalR Hub and web-sockets for transmitting data without refreshing.
Although currently it loads all who are in this section upon admin/staff joining messages are visible only if they happen after arrival of the user/admin/staff
2.7. Facebook Login

Facebook login (registration) is implemented.
2.8. Pagination

Pagination in All Products and Manage(Admin) occure if items exceed fixed ammount.
If data-tables is in use pagination is not implemented additionally because it is included in the library already.
3. Structure and code:
3.1. Overall

Project follows MVC architecture (mostly) in general. It is service based.
For NavButtons ViewComponents are in use. For Login, Register and Recomendations RazorPages are in use. For everything else RazorViews + Controllers.

3.2. Visit it at: https://junjuria.azurewebsites.net/
