using DoAnTotNghiep.DTOs;
using DoAnTotNghiep.Model;
using DoAnTotNghiep.Repository;

namespace DoAnTotNghiep.Services
{
    public class CommentServices : ICommentServices
    {
        private readonly ICommentRepository _commentRepository;

        public CommentServices(ICommentRepository commentRepository)
        {
            _commentRepository = commentRepository;
        }


        public async Task<bool> AddComment(string username, CommentDTOs commentDTOs)
        {
            return await _commentRepository.AddComment(username, commentDTOs);
        }

        public async Task<bool> DeleteComment(string role, string username, string Idcomment)
        {
            return await _commentRepository.DeleteComment(role, username, Idcomment);
        }

        public async Task<List<CommentToShowDTOs>> GetAllComment(string UserName)
        {
            return await _commentRepository.GetAllComment(UserName);
        }

        public async Task<List<Comment>> GetCommentByMovie(string slugMovie)
        {
            return await _commentRepository.GetCommentByMovie(slugMovie);
        }

        public Task<List<Comment>> GetCommentReport()
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateComment(string username, CommentUpdateDTOs commentDTOs)
        {
            return await _commentRepository.UpdateComment(username,commentDTOs);
        }
    }
}
