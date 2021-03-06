using IntegrationTests.Factories;
using IntegrationTests.Models;
using NUnit.Framework;
using RestSharp;
using System;

namespace IntegrationTests.Tests
{
    [TestFixture]
    public class IntegrationTests : IntegrationBaseTests
    {
        private RestClient _restClient;
        private Author _author;
        private Author _postedAuthor;
        private Book _book;
        private Book _postedBook;
        private IRestResponse _postNewAuthor;
        private IRestResponse _deletePostedAuthor;
        private IRestResponse _postNewBook;
        private IRestResponse _deletePostedBook;

        [SetUp]
        public void SetUp()
        {
            _restClient = new RestClient();
            _restClient.BaseUrl = new Uri("https://libraryjuly.azurewebsites.net");
            _author = AuthorFactory.CreateAuthorWithId();
            _book = BookFactory.CreateBook();
            
            _postNewAuthor = PostNewAuthor(_author, _restClient);
            _postedAuthor = Author.FromJson(_postNewAuthor.Content);
            _postNewBook = PostBookForAuthor(_postedAuthor, _book, _restClient);
            _postedBook = Book.FromJson(_postNewBook.Content);
        }

        [TearDown]
        public void TearDown()
        {
            _deletePostedBook = DeleteBookForAuthor(_postedAuthor, _postedBook, _restClient);
            _deletePostedAuthor = DeleteAnAuthor(_postedAuthor, _restClient);
        }

        [Test]
        public void Test1_PostAuthor()
        {
            Assert.IsTrue(_postNewAuthor.IsSuccessful);
        }

        [Test]
        public void Test2_GetAuthor()
        {
            var responseAfterPostingAnAuthor = GetAuthor($"/api/authors/{_postedAuthor.Id}", _restClient);

            Assert.AreEqual(responseAfterPostingAnAuthor.FirstName, _postedAuthor.FirstName);
            Assert.AreEqual(responseAfterPostingAnAuthor.LastName, _postedAuthor.LastName);
            Assert.AreEqual(responseAfterPostingAnAuthor.Age, _postedAuthor.Age);
        }

        [Test]
        public void Test3_PostInvalidAuthor()
        {
            var invalidAuthor = new Author() { DateOfBirth = "Invalid Date of birth" };

            var response = PostNewAuthor(invalidAuthor, _restClient);
            
            Assert.IsFalse(response.IsSuccessful);
        }

        [Test]
        public void Test4_PostBookForAuthor()
        {
            Assert.IsTrue(_postNewBook.IsSuccessful);
        }

        [Test]
        public void Test5_GetBookForAuthor()
        {
            var responseAfterGetBookForAuthor = GetBookForAuthor(_postedAuthor, _postedBook, _restClient);

            var returnedBook = Book.FromJson(responseAfterGetBookForAuthor.Content);

            Assert.IsTrue(responseAfterGetBookForAuthor.IsSuccessful);
            Assert.AreEqual(_postedBook.Id, returnedBook.Id);
            Assert.AreEqual(_postedBook.Title, returnedBook.Title);
            Assert.AreEqual(_postedBook.Description, returnedBook.Description);
        }

        [Test]
        public void Test6_PostInvalidBookForAuthor()
        {
            var invalidBook = new Book { AuthorId = Guid.Empty };

            var response = PostBookForAuthor(_postedAuthor, invalidBook, _restClient);

            Assert.IsFalse(response.IsSuccessful);
        }

        [Test]
        public void Test7_DeleteBookForAuthor()
        {
            var responseAfterDeletingABookForAuthor = DeleteBookForAuthor(_postedAuthor, _postedBook, _restClient);

            Assert.IsTrue(responseAfterDeletingABookForAuthor.IsSuccessful);
            Assert.AreEqual("NoContent", responseAfterDeletingABookForAuthor.StatusCode.ToString());
        }

        [Test]
        public void Test8_DeleteAnAuthor()
        {
            var responseAfterDeletingAnAuthor = DeleteAnAuthor(_postedAuthor, _restClient);

            Assert.IsTrue(responseAfterDeletingAnAuthor.IsSuccessful);
            Assert.AreEqual("NoContent", responseAfterDeletingAnAuthor.StatusCode.ToString());
        }
    }
}