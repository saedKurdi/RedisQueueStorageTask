namespace RedisOMDBTask.MVCApp.Dtos;
public class OMDBRepsonseDto
{
    public List<OMDBMovieDto>? Search {  get; set; }
    public string? TotalResults {  get; set; }
    public string? Response {  get; set; }
}
