using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonHelpers;
/// <summary>
/// An Object Representing the Result of an Operation . Either Success with a return TResult or Failure with a message or exception
/// </summary>
public class OperationResult<TResult>
{
    public TResult? Result { get; private set; } = default;

    /// <summary>
    /// The Context Failure => where it Happened
    /// </summary>
    public string FailureContext { get; private set; } = string.Empty;

    private string failureMessage = string.Empty;
    /// <summary>
    /// The Message of the Operation
    /// </summary>
    public string FailureMessage
    {
        get => $"{FailureContext}{(string.IsNullOrWhiteSpace(FailureContext) ? string.Empty : Environment.NewLine)}{failureMessage}";
        private set => failureMessage = value;
    }

    /// <summary>
    /// A Custom Message to Pass along with the Operation
    /// </summary>
    public string TagMessage { get; private set; } = string.Empty;

    /// <summary>
    /// The Exception if Any
    /// </summary>
    public Exception? Exception { get; private set; }

    /// <summary>
    /// Weather the operation was Successful
    /// </summary>
    public bool IsSuccessful { get; }
    public bool HasFailed { get => !IsSuccessful; }

    protected OperationResult(string message)
    {
        IsSuccessful = false;
        FailureMessage = message;
    }

    protected OperationResult(Exception ex)
    {
        IsSuccessful = false;
        Exception = ex;
        FailureMessage = ex.Message;
    }
    protected OperationResult(TResult result)
    {
        Result = result;
        IsSuccessful = true;
    }

    public static OperationResult<TResult> Success(TResult result)
    {
        return new OperationResult<TResult>(result);
    }

    public static OperationResult<TResult> Failure(string message)
    {
        return new OperationResult<TResult>(message);
    }
    public static OperationResult<TResult> Failure(Exception ex)
    {
        return new OperationResult<TResult>(ex);
    }

    /// <summary>
    /// Sets a Custom Message to be propagates along with the Operation
    /// </summary>
    /// <param name="message">The Message</param>
    /// <returns></returns>
    public OperationResult<TResult> SetTagMessage(string message)
    {
        TagMessage = message;
        return this;
    }

    /// <summary>
    /// Sets or changes the Failure Context
    /// </summary>
    /// <param name="message">The Context of the Failure (Where it happened)</param>
    /// <returns>The Operation</returns>
    public OperationResult<TResult> SetFailureContext(string message)
    {
        FailureContext = message;
        return this;
    }

}

/// <summary>
/// An Object Representing the Result of an Operation . Either Success or Failure with a message or exception
/// </summary>
public class OperationResult
{
    public bool IsSuccessful { get; }
    public bool HasFailed { get => !IsSuccessful; }

    /// <summary>
    /// The Context Failure => where it Happened
    /// </summary>
    public string FailureContext { get; private set; } = string.Empty;

    private string failureMessage = string.Empty;
    /// <summary>
    /// The Message of the Operation
    /// </summary>
    public string FailureMessage 
    {
        get => $"{FailureContext}{(string.IsNullOrWhiteSpace(FailureContext) ? string.Empty : Environment.NewLine)}{failureMessage}";
        private set => failureMessage = value;
    } 

    /// <summary>
    /// A Custom Message to Pass along with the Operation
    /// </summary>
    public string TagMessage { get; private set; } = string.Empty;

    /// <summary>
    /// The Exception if Any
    /// </summary>
    public Exception? Exception { get; private set; }

    protected OperationResult(string message)
    {
        IsSuccessful = false;
        FailureMessage = message;
    }
    protected OperationResult(Exception ex)
    {
        IsSuccessful = false;
        Exception = ex;
        FailureMessage = ex.Message;
    }
    protected OperationResult()
    {
        IsSuccessful = true;
    }

    public static OperationResult Success()
    {
        return new OperationResult();
    }

    public static OperationResult Failure(string message)
    {
        return new OperationResult(message);
    }
    public static OperationResult Failure(Exception ex)
    {
        return new OperationResult(ex);
    }
    /// <summary>
    /// Sets a Custom Message to be propagates along with the Operation
    /// </summary>
    /// <param name="message">The Message</param>
    /// <returns></returns>
    public OperationResult SetTagMessage(string message)
    {
        TagMessage = message;
        return this;
    }

    /// <summary>
    /// Sets or changes the Failure Context
    /// </summary>
    /// <param name="message">The Context of the Failure (Where it happened)</param>
    /// <returns>The Operation</returns>
    public OperationResult SetFailureContext(string message)
    {
        FailureContext = message;
        return this;
    }

}
