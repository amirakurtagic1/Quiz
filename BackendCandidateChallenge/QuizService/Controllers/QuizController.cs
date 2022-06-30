using System.Collections.Generic;
using System.Data;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using QuizService.Model;
using QuizService.Services.Interfaces;
using System.Threading.Tasks;

namespace QuizService.Controllers;

[Route("api/quizzes")]
public class QuizController : Controller
{
    private readonly IQuizService _quizService;

    public QuizController(IQuizService quizService)
    {
        this._quizService = quizService;
    }

    // GET api/quizzes
    [HttpGet]
    public async Task<IEnumerable<QuizResponseModel>> Get()
    {
        return await _quizService.GetAsync();
    }

    // GET api/quizzes/5
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var quiz = await _quizService.GetById(id);
        if (quiz == null) return NotFound();
        return this.Ok(quiz);
    }

    // POST api/quizzes
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] QuizCreateModel value)
    {
        var id = await _quizService.CreateAsync(value);
        return Created($"/api/quizzes/{id}", null);
    }

    // PUT api/quizzes/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] QuizUpdateModel value)
    {
        int rowsUpdated = await _quizService.UpdateAsync(id, value);
        if (rowsUpdated == 0)
            return NotFound();
        return NoContent();
    }

    // DELETE api/quizzes/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        int rowsDeleted = await _quizService.DeleteAsync(id);
        if (rowsDeleted == 0)
            return NotFound();
        return NoContent();
    }

    // POST api/quizzes/5/questions
    [HttpPost]
    [Route("{id}/questions")]
    public async Task<IActionResult> PostQuestion(int id, [FromBody] QuestionCreateModel value)
    {
        var questionId = await _quizService.CreateQuestionAsync(id, value);
        if (questionId == null)
            return NotFound();
        return Created($"/api/quizzes/{id}/questions/{questionId}", null);
    }

    // PUT api/quizzes/5/questions/6
    [HttpPut("{id}/questions/{qid}")]
    public async Task<IActionResult> PutQuestion(int id, int qid, [FromBody] QuestionUpdateModel value)
    {
        int rowsUpdated = await _quizService.UpdateQuestionAsync(qid, value);
        if (rowsUpdated == 0)
            return NotFound();
        return NoContent();
    }

    // DELETE api/quizzes/5/questions/6
    [HttpDelete]
    [Route("{id}/questions/{qid}")]
    public async Task<IActionResult> DeleteQuestion(int id, int qid)
    {
        await _quizService.DeleteQuestionAsync(qid);
        return NoContent();
    }

    // POST api/quizzes/5/questions/6/answers
    [HttpPost]
    [Route("{id}/questions/{qid}/answers")]
    public async Task<IActionResult> PostAnswer(int id, int qid, [FromBody] AnswerCreateModel value)
    {
        var answerId = await _quizService.CreateAnswerAsync(id, qid, value);
        return Created($"/api/quizzes/{id}/questions/{qid}/answers/{answerId}", null);
    }

    // PUT api/quizzes/5/questions/6/answers/7
    [HttpPut("{id}/questions/{qid}/answers/{aid}")]
    public async Task<IActionResult> PutAnswer(int id, int qid, int aid, [FromBody] AnswerUpdateModel value)
    {
        int rowsUpdated = await _quizService.UpdateAnswerAsync(aid, value);
        if (rowsUpdated == 0)
            return NotFound();
        return NoContent();
    }

    // DELETE api/quizzes/5/questions/6/answers/7
    [HttpDelete]
    [Route("{id}/questions/{qid}/answers/{aid}")]
    public async Task<IActionResult> DeleteAnswer(int id, int qid, int aid)
    {
        await _quizService.DeleteAnswerAsync(aid);
        return NoContent();
    }
}