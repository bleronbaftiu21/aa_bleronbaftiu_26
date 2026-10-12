using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace AlgoritmeTeAvancuara.java2
{
    public static class Program
    {
        private static readonly JsonSerializerOptions Options = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            WriteIndented = true
        };

        public static void Run()
        {
            // Te dhanat

            string json = """
{
  "start_question_id": "q1",
  "questions": [
    {
      "id": "q1",
      "text": "Do you own a car?",
      "type": "radio",
      "answers": [
        { "id": "a1_yes", "text": "Yes", "next_question_id": "q2" },
        { "id": "a1_no", "text": "No", "next_question_id": "q5" }
      ]
    },
    {
      "id": "q2",
      "text": "What type of car do you own?",
      "type": "select",
      "answers": [
        { "id": "a2_sedan", "text": "Sedan", "next_question_id": "q3" },
        { "id": "a2_suv", "text": "SUV", "next_question_id": "q3" },
        { "id": "a2_truck", "text": "Truck", "next_question_id": "q4" },
        { "id": "a2_sports", "text": "Sports Car", "next_question_id": "q4" }
      ]
    },
    {
      "id": "q3",
      "text": "How many passengers can your vehicle seat?",
      "type": "radio",
      "answers": [
        { "id": "a3_2to5", "text": "2-5", "next_question_id": "q6" },
        { "id": "a3_6plus", "text": "6+", "next_question_id": "q6" }
      ]
    },
    {
      "id": "q4",
      "text": "Do you use your vehicle for work purposes?",
      "type": "radio",
      "answers": [
        { "id": "a4_yes", "text": "Yes", "next_question_id": "q7" },
        { "id": "a4_no", "text": "No", "next_question_id": "q6" }
      ]
    },
    {
      "id": "q5",
      "text": "What is your primary mode of transportation?",
      "type": "select",
      "answers": [
        { "id": "a5_public", "text": "Public Transit", "next_question_id": "q6" },
        { "id": "a5_bike", "text": "Bicycle", "next_question_id": "q6" },
        { "id": "a5_walk", "text": "Walking", "next_question_id": "q6" }
      ]
    },
    {
      "id": "q6",
      "text": "What is your annual commute distance?",
      "type": "text",
      "next_question_id": "q8"
    },
    {
      "id": "q7",
      "text": "What percentage of your vehicle use is for work?",
      "type": "text",
      "next_question_id": "q8"
    },
    {
      "id": "q8",
      "text": "Are you satisfied with your current transportation situation?",
      "type": "radio",
      "answers": [
        { "id": "a8_yes", "text": "Yes", "next_question_id": null },
        { "id": "a8_no", "text": "No", "next_question_id": "q9" }
      ]
    },
    {
      "id": "q9",
      "text": "What would you like to improve?",
      "type": "text",
      "next_question_id": null
    }
  ]
}
""";

            var questionnaire =
                JsonSerializer.Deserialize<Questionnaire>(json, Options)
                ?? throw new Exception("JSON i pavlefshem.");

            Print("1. Rruga me e gjate:",
                Analyzer.FindLongestQuestionPath(questionnaire));

            Print("2. Rruga me e shkurter:",
                Analyzer.FindShortestQuestionPath(questionnaire));

            Print("3. Numri i te gjitha rrugeve:",
                Analyzer.CountAllPossiblePaths(questionnaire));

            Print("4. Pyetjet pas q2 / a2_truck:",
                Analyzer.FindReachableQuestions(
                    questionnaire, "q2", "a2_truck"));

            Print("5. Probabilitetet dhe pozicionet mesatare:",
                Analyzer.CalculateQuestionProbabilities(questionnaire));
        }

        private static void Print(string title, object result)
        {
            Console.WriteLine(title);
            Console.WriteLine(JsonSerializer.Serialize(result, Options));
            Console.WriteLine();
        }
    }

    // modelet
    public record Answer(
        string Id,
        string Text,
        string? NextQuestionId);

    public record Question(
        string Id,
        string Text,
        string Type,
        List<Answer>? Answers = null,
        string? NextQuestionId = null);

    public record Questionnaire(
        List<Question> Questions,
        string StartQuestionId);

    public record QuestionPath(
        int Length,
        string[] QuestionIds,
        string[] AnswerPath);

    public record ReachableQuestion(
        string QuestionId,
        string QuestionText,
        int MinDepth);

    public record QuestionProbability(
        string QuestionId,
        string QuestionText,
        double Probability,
        double ExpectedPosition);

    // algoritmet
    public static class Analyzer
    {
        private record Step(string? NextId, string? AnswerId);

        // Nderton indeksin dhe kontrollon te dhenat.
        private static Dictionary<string, Question> BuildIndex(
            Questionnaire questionnaire)
        {
            var index = new Dictionary<string, Question>(
                StringComparer.Ordinal);

            foreach (var question in questionnaire.Questions)
            {
                if (!index.TryAdd(question.Id, question))
                    throw new ArgumentException(
                        $"ID e dyfishuar: {question.Id}");

                if (question.Type is "radio" or "select")
                {
                    if (question.Answers is not { Count: > 0 })
                        throw new ArgumentException(
                            $"Pyetja {question.Id} duhet te kete pergjigje.");

                    if (question.Answers.Select(a => a.Id)
                        .Distinct(StringComparer.Ordinal).Count()
                        != question.Answers.Count)
                    {
                        throw new ArgumentException(
                            $"ID pergjigjesh te dyfishuara te {question.Id}.");
                    }
                }
                else if (question.Type is not ("text" or "checkbox"))
                {
                    throw new ArgumentException(
                        $"Tip i panjohur: {question.Type}");
                }
            }

            if (!index.ContainsKey(questionnaire.StartQuestionId))
                throw new ArgumentException(
                    "Pyetja fillestare nuk ekziston.");

            foreach (var question in questionnaire.Questions)
            {
                foreach (var step in NextSteps(question))
                {
                    if (step.NextId is not null
                        && !index.ContainsKey(step.NextId))
                    {
                        throw new ArgumentException(
                            $"Pyetja {question.Id} referon ID qe mungon: " +
                            step.NextId);
                    }
                }
            }

            return index;
        }

        private static IEnumerable<Step> NextSteps(Question question)
        {
            if (question.Type is "radio" or "select")
            {
                foreach (var answer in question.Answers!)
                    yield return new Step(
                        answer.NextQuestionId, answer.Id);
            }
            else
            {
                yield return new Step(question.NextQuestionId, null);
            }
        }

        //problemi 1
        public static QuestionPath FindLongestQuestionPath(
            Questionnaire questionnaire)
        {
            return FindBestPath(questionnaire, longest: true);
        }

        //problemi 2
        public static QuestionPath FindShortestQuestionPath(
            Questionnaire questionnaire)
        {
            return FindBestPath(questionnaire, longest: false);
        }

        private static QuestionPath FindBestPath(
            Questionnaire questionnaire, bool longest)
        {
            var index = BuildIndex(questionnaire);
            var active = new HashSet<string>(StringComparer.Ordinal);

            QuestionPath? Visit(string? id)
            {
                if (id is null)
                    return new QuestionPath(0, [], []);

                if (!active.Add(id))
                    return null;

                QuestionPath? best = null;

                foreach (var step in NextSteps(index[id]))
                {
                    var suffix = Visit(step.NextId);

                    if (suffix is null)
                        continue;

                    var candidate = new QuestionPath(
                        suffix.Length + 1,
                        [id, .. suffix.QuestionIds],
                        step.AnswerId is null
                            ? suffix.AnswerPath
                            : [step.AnswerId, .. suffix.AnswerPath]);

                    if (best is null || (longest
                        ? candidate.Length > best.Length
                        : candidate.Length < best.Length))
                    {
                        best = candidate;
                    }
                }

                active.Remove(id);

                return best;
            }

            return Visit(questionnaire.StartQuestionId)
                ?? throw new InvalidOperationException(
                    "Nuk ka rruge pa perseritje qe perfundon ne null.");
        }

        private static bool HasCycle(
            Dictionary<string, Question> index)
        {
            var colors = new Dictionary<string, int>(
                StringComparer.Ordinal);

            bool Visit(string id)
            {
                if (colors.TryGetValue(id, out int color))
                    return color == 1;

                colors[id] = 1;

                foreach (var step in NextSteps(index[id]))
                {
                    if (step.NextId is not null && Visit(step.NextId))
                        return true;
                }

                colors[id] = 2;
                return false;
            }

            var ids = index.Keys.ToArray();

            bool CheckComponent(int position)
            {
                if (position == ids.Length)
                    return false;

                return Visit(ids[position])
                    || CheckComponent(position + 1);
            }

            return CheckComponent(0);
        }

        //problemi 3
        public static long CountAllPossiblePaths(
            Questionnaire questionnaire)
        {
            var index = BuildIndex(questionnaire);

            if (HasCycle(index))
                return -1;

            var memo = new Dictionary<string, long>(
                StringComparer.Ordinal);

            long Count(string? id)
            {
                if (id is null)
                    return 1;

                if (memo.TryGetValue(id, out long cached))
                    return cached;

                long total = 0;

                foreach (var step in NextSteps(index[id]))
                    total = checked(total + Count(step.NextId));

                memo[id] = total;
                return total;
            }

            return Count(questionnaire.StartQuestionId);
        }

        //problemi 4
        public static ReachableQuestion[] FindReachableQuestions(
            Questionnaire questionnaire,
            string questionId,
            string answerId)
        {
            var index = BuildIndex(questionnaire);

            if (!index.TryGetValue(questionId, out var question)
                || question.Type is not ("radio" or "select"))
            {
                return [];
            }

            var answer = question.Answers!
                .FirstOrDefault(a => a.Id == answerId);

            if (answer is null || answer.NextQuestionId is null)
                return [];

            var depths = new Dictionary<string, int>(
                StringComparer.Ordinal);

            void Visit(string? id, int depth)
            {
                if (id is null)
                    return;

                if (depths.TryGetValue(id, out int previous)
                    && previous <= depth)
                {
                    return;
                }

                depths[id] = depth;

                foreach (var step in NextSteps(index[id]))
                    Visit(step.NextId, depth + 1);
            }

            Visit(answer.NextQuestionId, 1);

            return depths
                .Select(pair => new ReachableQuestion(
                    pair.Key,
                    index[pair.Key].Text,
                    pair.Value))
                .OrderBy(r => r.MinDepth)
                .ThenBy(r => r.QuestionId, StringComparer.Ordinal)
                .ToArray();
        }

        //problemi 5
        public static QuestionProbability[]
            CalculateQuestionProbabilities(
                Questionnaire questionnaire)
        {
            var index = BuildIndex(questionnaire);

            if (HasCycle(index))
            {
                throw new InvalidOperationException(
                    "Probabilitetet kerkojne nje DAG, pa cikle.");
            }

            var probabilities = new Dictionary<string, double>(
                StringComparer.Ordinal);

            var weightedPositions = new Dictionary<string, double>(
                StringComparer.Ordinal);

            void Visit(string? id, double probability, int position)
            {
                if (id is null)
                    return;

                probabilities[id] =
                    probabilities.GetValueOrDefault(id) + probability;

                weightedPositions[id] =
                    weightedPositions.GetValueOrDefault(id)
                    + probability * position;

                var question = index[id];

                double branchProbability =
                    question.Type is "radio" or "select"
                        ? probability / question.Answers!.Count
                        : probability;

                foreach (var step in NextSteps(question))
                {
                    Visit(
                        step.NextId,
                        branchProbability,
                        position + 1);
                }
            }

            Visit(questionnaire.StartQuestionId, 1.0, 1);

            return index.Values
                .Select(question =>
                {
                    double probability =
                        probabilities.GetValueOrDefault(question.Id);

                    double expectedPosition = probability == 0
                        ? 0
                        : weightedPositions[question.Id] / probability;

                    return new QuestionProbability(
                        question.Id,
                        question.Text,
                        Math.Round(
                            probability, 4,
                            MidpointRounding.AwayFromZero),
                        Math.Round(
                            expectedPosition, 4,
                            MidpointRounding.AwayFromZero));
                })
                .OrderByDescending(r => r.Probability)
                .ThenBy(r => r.QuestionId, StringComparer.Ordinal)
                .ToArray();
        }
    }
}